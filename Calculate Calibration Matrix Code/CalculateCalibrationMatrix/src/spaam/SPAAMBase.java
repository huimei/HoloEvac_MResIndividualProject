// DLT related code taken from https://github.com/krm104/AndroidSPAAM, modified by Hui Mei for 3D - 3D estimation instead of 2D- 3D

package spaam;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.List;

import org.opencv.calib3d.Calib3d;
import org.opencv.core.Core;
import org.opencv.core.CvType;
import org.opencv.core.Mat;

//Import needed for the Linear Algebra and Matrix related math functions//
import Jama.Matrix;
import RANSAC.RANSAC2D;
import matrix.NoSquareException;

public class SPAAMBase {
	static public class SPAAM_SVD {

		public SPAAM_SVD() {

		}

		static public class Correspondence_Pair {
			public Correspondence_Pair() {

			}

			public Correspondence_Pair(double x1, double y1, double z1, double x2, double y2, double z2) {
				worldPoint.set(0, 0, x1);
				worldPoint.set(0, 1, y1);
				worldPoint.set(0, 2, z1);
				screenPoint.set(0, 0, x2);
				screenPoint.set(0, 1, y2);
				screenPoint.set(0, 2, z2);
			}

			public Matrix worldPoint = new Matrix(1, 3);
			public Matrix screenPoint = new Matrix(1, 3);
		}

		public List<Correspondence_Pair> corr_points = new ArrayList<Correspondence_Pair>();

		// Normalization Components for World Points
		private Matrix fromShift = new Matrix(1, 3);
		private Matrix fromScale = new Matrix(1, 3);
		// Normalization Components for Screen Points
		private Matrix toShift = new Matrix(1, 3);
		private Matrix toScale = new Matrix(1, 3);
		// Normalization Matrix for World Points
		private Matrix modMatrixWorld = new Matrix(4, 4);
		// Normalization Matrix for Screen Points
		private Matrix modMatrixScreen = new Matrix(4, 4);

		// Final 3 x 4 Projection Matrix//
		public Matrix Proj3x4 = new Matrix(3, 4);
		public double[] projMat3x4 = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
		
		public DecimalFormat df = new DecimalFormat("#.#####");

		private Matrix element_div(Matrix m1, Matrix m2) {
			Matrix result = null;
			if (m1.getColumnDimension() == m1.getColumnDimension())
				if (m1.getRowDimension() == m2.getRowDimension()) {
					result = new Matrix(m1.getRowDimension(), m1.getColumnDimension());
					for (int i = 0; i < m1.getRowDimension(); i++)
						for (int j = 0; j < m1.getColumnDimension(); j++) {
							result.set(i, j, m1.get(i, j) / m2.get(i, j));
						}
				}

			return result;
		}
		// Normalize data
		private void estimateNormalizationParameters() {
			System.out.println("In estimateNormalizationParameters");
			
			// determine the number of points to be normalized
			double n_pts = corr_points.size();

			fromShift = new Matrix(1, 3);
			fromScale = new Matrix(1, 3);
			toShift = new Matrix(1, 3);
			toScale = new Matrix(1, 3);

			// compute mean and mean of square
			for (int i = 0; i < corr_points.size(); ++i) {
				fromShift = fromShift.plus(corr_points.get(i).worldPoint);
				Matrix tempscale = new Matrix(1, 3);

				tempscale.set(0, 0, corr_points.get(i).worldPoint.get(0, 0) * corr_points.get(i).worldPoint.get(0, 0));
				tempscale.set(0, 1, corr_points.get(i).worldPoint.get(0, 1) * corr_points.get(i).worldPoint.get(0, 1));
				tempscale.set(0, 2, corr_points.get(i).worldPoint.get(0, 2) * corr_points.get(i).worldPoint.get(0, 2));
				fromScale = fromScale.plus(tempscale);

				toShift = toShift.plus(corr_points.get(i).screenPoint);
				Matrix temptscale = new Matrix(1, 3);
				temptscale.set(0, 0, corr_points.get(i).screenPoint.get(0, 0) * corr_points.get(i).screenPoint.get(0, 0));
				temptscale.set(0, 1, corr_points.get(i).screenPoint.get(0, 1) * corr_points.get(i).screenPoint.get(0, 1));
				temptscale.set(0, 2, corr_points.get(i).screenPoint.get(0, 2) * corr_points.get(i).screenPoint.get(0, 2));
				toScale = toScale.plus(temptscale);
			}
			fromShift = fromShift.times((1.0) / n_pts);
			fromScale = fromScale.times((1.0) / n_pts);
			toShift = toShift.times((1.0) / n_pts);
			toScale = toScale.times((1.0) / n_pts);

			// compute standard deviation
			for (int i = 0; i < 3; i++) {
				fromScale.set(0, i, Math.sqrt(fromScale.get(0, i) - (fromShift.get(0, i) * fromShift.get(0, i))));
			}
			for (int i = 0; i < 3; i++) {
				toScale.set(0, i, Math.sqrt(toScale.get(0, i) - (toShift.get(0, i) * toShift.get(0, i))));
			}
			
			System.out.println("End estimateNormalizationParameters");
		}

		// transform the result of the SVD function back into the proper range of values
		private void generateNormalizationMatrix() {
			System.out.println("In generateNormalizationMatrix");
			
			modMatrixWorld = new Matrix(4, 4);
			modMatrixScreen = new Matrix(4, 4);

			// create homogeneous matrix//
			modMatrixWorld.set(3, 3, 1.0);
			modMatrixScreen.set(3, 3, 1.0);

			for (int i = 0; i < 3; i++) {
				modMatrixScreen.set(i, i, toScale.get(0, i));
				modMatrixScreen.set(i, 3, toShift.get(0, i));
			}

			for (int i = 0; i < 3; i++) {
				modMatrixWorld.set(i, i, (1.0) / fromScale.get(0, i));
				modMatrixWorld.set(i, 3, -modMatrixWorld.get(i, i) * fromShift.get(0, i));
			}
			
			System.out.println("End of generateNormalizationMatrix");
		}

		public Matrix ransac2D (String operation) throws NoSquareException {
			RANSAC2D ransac2D = new RANSAC2D();
			corr_points = ransac2D.performRANSAC2D(corr_points);
			
			if (corr_points != null) {
				if (operation.equalsIgnoreCase("LS")) {
					return leastSquareOperation(corr_points);
				} else if (operation.equalsIgnoreCase("DLT")) {
					return projectionDLTImpl(corr_points);
				} else if (operation.equalsIgnoreCase("DLT2")) {
					return projectionDLTImpl2(corr_points);
				}else {
					return null;
				}
			}
			return null;
		}
		
		public Matrix openCVEstimateAffine(List<Correspondence_Pair> corr_points) {
			System.loadLibrary(Core.NATIVE_LIBRARY_NAME);
			
			Mat srcMat = new Mat(corr_points.size(), 3, CvType.CV_64F);
			
			Mat dstMat = new Mat(corr_points.size(), 3, CvType.CV_64F);
			
			for (int i = 0; i < corr_points.size(); i++) {
				double [] worldPointArray = new double [3];
				double [] screenPointArray = new double [3];
				for (int j = 0; j < 3; j++) {
					worldPointArray[j] = corr_points.get(i).worldPoint.get(0, j);
					screenPointArray[j] = corr_points.get(i).screenPoint.get(0, j);
				}
				
				srcMat.put(i, 0, worldPointArray);
				dstMat.put(i, 0, screenPointArray);
			}
			
			Mat outMat = new Mat();
			
			Mat inliersMat = new Mat();
			
			//Calib3d.estimateAffine3D(srcMat, dstMat, outMat, inliersMat);
			Calib3d.estimateAffine3D(srcMat, dstMat, outMat, inliersMat, 5, 0.99);
			
			Matrix result = new Matrix(4, 4);
			for (int i = 0; i < outMat.rows(); i++) {
				for (int j = 0; j < outMat.cols(); j++){
					if (i < 3) {
						double[] currentRow = outMat.get(i, j);
						
						for (int k = 0; k < currentRow.length; k++) {
							
							result.set(i, j, currentRow[k]);
						}
					}
				}
			}
			result.set(3, 3, 1.00);

			return result;
		}

		public Matrix leastSquareOperation(List<Correspondence_Pair> corr_points) throws NoSquareException{
			System.out.println("In leastSquareOperation");
			
			if (corr_points.size() < 6){
				System.out.println("corr_points.size() < 6");
				return null;
			}
			
			Matrix P = new Matrix (corr_points.size(), 4); //Defined
			Matrix Q = new Matrix (corr_points.size(), 4); //Measured
			Matrix Q_Star = new Matrix(4, corr_points.size());
			
			for (int i = 0; i < corr_points.size(); i++) {
				for (int j = 0; j < 3; j++){
					P.set(i, j, corr_points.get(i).screenPoint.get(0, j));
					Q.set(i, j, corr_points.get(i).worldPoint.get(0, j));
					Q_Star.set(j, i, corr_points.get(i).worldPoint.get(0, j));
				}
				P.set(i, 3, 1.0f);
				Q.set(i, 3, 1.0f);
				Q_Star.set(3, i, 1.0f);
			}
	
			Matrix Q_temp = Q_Star.times(Q);
			
			Q_temp = Q_temp.inverse();
			
			Matrix Q_pinv = Q_temp.times(Q_Star);
			
			Matrix result = Q_pinv.times(P);
			Matrix finalResult = new Matrix (4, 4);

			for (int i = 0; i < result.getColumnDimension(); i++){
				for (int j = 0; j < result.getRowDimension(); j++){
					finalResult.set(i, j, result.get(j, i));
				}
			}
			
			System.out.println("leastSquareOperation result: ");
			finalResult.print(0, 3);
			
			return finalResult;
		}
		
		public Matrix projectionDLTImpl2(List<Correspondence_Pair> corr_points){
			
			System.out.println("In projectionDLTImpl2");
			
			estimateNormalizationParameters();
			
			Matrix A = new Matrix(3 * corr_points.size(), 16);
			
			for (int i = 0; i < corr_points.size(); i++) {
				Matrix to = element_div(corr_points.get(i).screenPoint.minus(toShift), toScale);
				
				Matrix from = element_div(corr_points.get(i).worldPoint.minus(fromShift), fromScale);

				A.set(i * 3, 0, -from.get(0,0));
				A.set(i * 3, 1, -from.get(0,1));
				A.set(i * 3, 2, -from.get(0,2));
				A.set(i * 3, 3, -1);
				A.set(i * 3, 4, 0);
				A.set(i * 3, 5, 0);
				A.set(i * 3, 6, 0);
				A.set(i * 3, 7, 0);
				A.set(i * 3, 8, 0);
				A.set(i * 3, 9, 0);
				A.set(i * 3, 10, 0);
				A.set(i * 3, 11, 0);
				A.set(i * 3, 12, to.get(0, 0) * from.get(0, 0));
				A.set(i * 3, 13, to.get(0, 0) * from.get(0, 1));
				A.set(i * 3, 14, to.get(0, 0) * from.get(0, 2));
				A.set(i * 3, 15, to.get(0, 0));
				
				A.set(i * 3 + 1, 0, 0);
				A.set(i * 3 + 1, 1, 0);
				A.set(i * 3 + 1, 2, 0);
				A.set(i * 3 + 1, 3, 0);
				A.set(i * 3 + 1, 4, -from.get(0, 0));
				A.set(i * 3 + 1, 5, -from.get(0, 1));
				A.set(i * 3 + 1, 6, -from.get(0, 2));
				A.set(i * 3 + 1, 7, -1);
				A.set(i * 3 + 1, 8, 0);
				A.set(i * 3 + 1, 9, 0);
				A.set(i * 3 + 1, 10, 0);
				A.set(i * 3 + 1, 11, 0);
				A.set(i * 3 + 1, 12, to.get(0, 1) * from.get(0, 0));
				A.set(i * 3 + 1, 13, to.get(0, 1) * from.get(0, 1));
				A.set(i * 3 + 1, 14, to.get(0, 1) * from.get(0, 2));
				A.set(i * 3 + 1, 15, to.get(0, 1));
				
				A.set(i * 3 + 2, 0, 0);
				A.set(i * 3 + 2, 1, 0);
				A.set(i * 3 + 2, 2, 0);
				A.set(i * 3 + 2, 3, 0);
				A.set(i * 3 + 2, 4, 0);
				A.set(i * 3 + 2, 5, 0);
				A.set(i * 3 + 2, 6, 0);
				A.set(i * 3 + 2, 7, 0);
				A.set(i * 3 + 2, 8, -from.get(0, 0));
				A.set(i * 3 + 2, 9, -from.get(0, 1));
				A.set(i * 3 + 2, 10, -from.get(0, 2));
				A.set(i * 3 + 2, 11, -1);
				A.set(i * 3 + 2, 12, to.get(0, 2) * from.get(0, 0));
				A.set(i * 3 + 2, 13, to.get(0, 2) * from.get(0, 1));
				A.set(i * 3 + 2, 14, to.get(0, 2) * from.get(0, 2));
				A.set(i * 3 + 2, 15, to.get(0, 2));			
			}
			Matrix Vt = new Matrix(16, 16);
			Vt = A.svd().getV().transpose();

			Matrix result = new Matrix (4, 4);
			// copy result to 3x4 matrix
			result.set(0, 0, Vt.get(15, 0));
			result.set(0, 1, Vt.get(15, 1));
			result.set(0, 2, Vt.get(15, 2));
			result.set(0, 3, Vt.get(15, 3));
			result.set(1, 0, Vt.get(15, 4));
			result.set(1, 1, Vt.get(15, 5));
			result.set(1, 2, Vt.get(15, 6));
			result.set(1, 3, Vt.get(15, 7));
			result.set(2, 0, Vt.get(15, 8));
			result.set(2, 1, Vt.get(15, 9));
			result.set(2, 2, Vt.get(15, 10));
			result.set(2, 3, Vt.get(15, 11));
			result.set(3, 0, Vt.get(15, 12));
			result.set(3, 1, Vt.get(15, 13));
			result.set(3, 2, Vt.get(15, 14));
			result.set(3, 3, Vt.get(15, 15));
			
			// reverse normalization
			generateNormalizationMatrix();
						
			Matrix toCorrect = new Matrix((modMatrixScreen.getArray()));			
			Matrix Ptemp = new Matrix(4, 4);
			Ptemp = toCorrect.times(result);

			Matrix fromCorrect = new Matrix((modMatrixWorld.getArray()));
			result = Ptemp.times(fromCorrect);
			
			result.print(0, 3);

			return result;
		}

		public Matrix projectionDLTImpl(List<Correspondence_Pair> corr_points) {
			System.out.println("In projectionDLTImpl");
			
			// minimum of 6 correspondence points required to solve//
			if (corr_points.size() < 6)
				return null;

			// normalize input points
			estimateNormalizationParameters();

			// construct equation system
			Matrix A = new Matrix(3 * corr_points.size(), 16);
			
			//for (int i = 0; i < 20; i++) {
			for (int i = 0; i < corr_points.size(); i++) {
				Matrix to = element_div(corr_points.get(i).screenPoint.minus(toShift), toScale);
				
				Matrix from = element_div(corr_points.get(i).worldPoint.minus(fromShift), fromScale);

				A.set(i * 3, 0, -from.get(0,0));
				A.set(i * 3, 1, -from.get(0,1));
				A.set(i * 3, 2, -from.get(0,2));
				A.set(i * 3, 3, -1);
				A.set(i * 3, 4, 0);
				A.set(i * 3, 5, 0);
				A.set(i * 3, 6, 0);
				A.set(i * 3, 7, 0);
				A.set(i * 3, 8, 0);
				A.set(i * 3, 9, 0);
				A.set(i * 3, 10, 0);
				A.set(i * 3, 11, 0);
				A.set(i * 3, 12, to.get(0, 0) * from.get(0, 0));
				A.set(i * 3, 13, to.get(0, 0) * from.get(0, 1));
				A.set(i * 3, 14, to.get(0, 0) * from.get(0, 2));
				A.set(i * 3, 15, to.get(0, 0));
				
				A.set(i * 3 + 1, 0, 0);
				A.set(i * 3 + 1, 1, 0);
				A.set(i * 3 + 1, 2, 0);
				A.set(i * 3 + 1, 3, 0);
				A.set(i * 3 + 1, 4, -from.get(0, 0));
				A.set(i * 3 + 1, 5, -from.get(0, 1));
				A.set(i * 3 + 1, 6, -from.get(0, 2));
				A.set(i * 3 + 1, 7, -1);
				A.set(i * 3 + 1, 8, 0);
				A.set(i * 3 + 1, 9, 0);
				A.set(i * 3 + 1, 10, 0);
				A.set(i * 3 + 1, 11, 0);
				A.set(i * 3 + 1, 12, to.get(0, 1) * from.get(0, 0));
				A.set(i * 3 + 1, 13, to.get(0, 1) * from.get(0, 1));
				A.set(i * 3 + 1, 14, to.get(0, 1) * from.get(0, 2));
				A.set(i * 3 + 1, 15, to.get(0, 1));
				
				A.set(i * 3 + 2, 0, 0);
				A.set(i * 3 + 2, 1, 0);
				A.set(i * 3 + 2, 2, 0);
				A.set(i * 3 + 2, 3, 0);
				A.set(i * 3 + 2, 4, 0);
				A.set(i * 3 + 2, 5, 0);
				A.set(i * 3 + 2, 6, 0);
				A.set(i * 3 + 2, 7, 0);
				A.set(i * 3 + 2, 8, -from.get(0, 0));
				A.set(i * 3 + 2, 9, -from.get(0, 1));
				A.set(i * 3 + 2, 10, -from.get(0, 2));
				A.set(i * 3 + 2, 11, -1);
				A.set(i * 3 + 2, 12, to.get(0, 2) * from.get(0, 0));
				A.set(i * 3 + 2, 13, to.get(0, 2) * from.get(0, 1));
				A.set(i * 3 + 2, 14, to.get(0, 2) * from.get(0, 2));
				A.set(i * 3 + 2, 15, to.get(0, 2));
				
				
			}
			
			// solve using SVD
			// Matrix s = new Matrix(1, 12);
			// Matrix U = new Matrix( 2 * corr_points.size(), 2 * corr_points.size() );			
			Matrix Vt = new Matrix(16, 16);
			Vt = A.svd().getV().transpose();

			Matrix result = new Matrix (4, 4);
			// copy result to 3x4 matrix
			result.set(0, 0, Vt.get(15, 0));
			result.set(0, 1, Vt.get(15, 1));
			result.set(0, 2, Vt.get(15, 2));
			result.set(0, 3, Vt.get(15, 3));
			result.set(1, 0, Vt.get(15, 4));
			result.set(1, 1, Vt.get(15, 5));
			result.set(1, 2, Vt.get(15, 6));
			result.set(1, 3, Vt.get(15, 7));
			result.set(2, 0, Vt.get(15, 8));
			result.set(2, 1, Vt.get(15, 9));
			result.set(2, 2, Vt.get(15, 10));
			result.set(2, 3, Vt.get(15, 11));
			result.set(3, 0, Vt.get(15, 12));
			result.set(3, 1, Vt.get(15, 13));
			result.set(3, 2, Vt.get(15, 14));
			result.set(3, 3, Vt.get(15, 15));

			
			// reverse normalization
			generateNormalizationMatrix();
			
			Matrix toCorrect = new Matrix((modMatrixScreen.getArray()));			
			Matrix Ptemp = new Matrix(4, 4);
			Ptemp = toCorrect.times(result);
			
			Matrix fromCorrect = new Matrix((modMatrixWorld.getArray()));
			result = Ptemp.times(fromCorrect);

			// normalize result to have a viewing direction of length 1 (optional)
			double fViewDirLen = Math.sqrt(result.get(3, 0) * result.get(3, 0) + result.get(3, 1) * result.get(3, 1)
					+ result.get(3, 2) * result.get(3, 2) + result.get(3, 3) * result.get(3, 3));

			// if first point is projected onto a negative z value, negate matrix
			Matrix p1st = new Matrix(corr_points.get(0).worldPoint.getArray());
			if (result.get(3, 0) * p1st.get(0, 0) + result.get(3, 1) * p1st.get(0, 1)
					+ result.get(3, 2) * p1st.get(0, 2) + result.get(3, 3) < 0)
				fViewDirLen = -fViewDirLen;

			result = result.times((1.0) / fViewDirLen);

			result.print(0, 3);

			return result;
		}
	}
}