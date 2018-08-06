import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.text.DateFormat;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import matrix.Matrix;
import matrix.MatrixMathematics;
import matrix.NoSquareException;
import spaam.SPAAMBase.SPAAM_SVD;
import spaam.SPAAMBase.SPAAM_SVD.Correspondence_Pair;

public class Main {
	static DecimalFormat df = new DecimalFormat("#.########");

	static DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
	static Date date = new Date();

	// Control Part
	static boolean screenPointCalib = true;
	static boolean addPoint = false;
	static int veriStartPoint = 80;
	static int currentVersion = 4;
	static String dataFileName = "MSingleStaticLeft_4";

	private static rawDataInArrayObject verificationData = new rawDataInArrayObject();

	private enum calibrationType {
		DLT, DLT2, LS, ES, RS2
	}
	
	private static String[] operationWithRANSAC = {"DLT", "DLT2", "LS"};

	public static void main(String[] args) throws IOException, NoSquareException {
		StringBuilder inputFileNameBuilder = new StringBuilder();
		inputFileNameBuilder.append("./data/");
		inputFileNameBuilder.append(dataFileName);
		inputFileNameBuilder.append(".csv");

		StringBuilder dirNameBuilder = new StringBuilder();
		dirNameBuilder.append("./Results/");
		dirNameBuilder.append(dateFormat.format(date));
		dirNameBuilder.append("/Version");
		dirNameBuilder.append(currentVersion);
		
		File directory = new File(dirNameBuilder.toString());
		if (!directory.exists()) {
			directory.mkdirs();
		}

		List<reprojectionErrorObject> reprojectionErrorList = new ArrayList<reprojectionErrorObject> ();
		for (calibrationType calibType : calibrationType.values()) {
			Jama.Matrix calibrationMatrix = new Jama.Matrix(4, 4);
			
			for (int size = 10; size < veriStartPoint; size += 10) {
				rawDataInArrayObject data = readCSV(inputFileNameBuilder.toString(), size);
				
				if (calibType.toString().equalsIgnoreCase("RS2")) {
					for (int i = 0; i < operationWithRANSAC.length; i++) {
						String secondOperation = operationWithRANSAC[i];
						
						calibrationMatrix = calculateCalibrationMatrix (data, calibType.toString(), secondOperation);
						
						StringBuilder customOperationType = new StringBuilder ();
						customOperationType.append(calibType.toString());
						customOperationType.append(" - ");
						customOperationType.append(secondOperation);
						
						String outputFileName = getOutputFileName (dirNameBuilder, customOperationType.toString(), size);
						reprojectionErrorObject reprojectionError = verificationOfCalibrationMatrix(calibrationMatrix, outputFileName);					
						
						if (reprojectionError != null) {
							reprojectionError.setOperationType(customOperationType.toString() + " - " + size);
							
							reprojectionErrorList.add(reprojectionError);
						}
					}
				}else {
					calibrationMatrix = calculateCalibrationMatrix (data, calibType.toString(), null);
					
					String outputFileName = getOutputFileName (dirNameBuilder, calibType.toString(), size);
					reprojectionErrorObject reprojectionError = verificationOfCalibrationMatrix(calibrationMatrix, outputFileName);
					
					if (reprojectionError != null) {
						reprojectionError.setOperationType(calibType.toString() + " - " + size);
						
						reprojectionErrorList.add(reprojectionError);
					}
				}
			}
			
			// Create summary
			StringBuilder outputFileName = new StringBuilder();
			outputFileName.append(dirNameBuilder.toString());
			outputFileName.append("/0Summary - ");
			outputFileName.append(dataFileName);
			outputFileName.append(".csv");
			
			BufferedWriter writer = new BufferedWriter(new FileWriter(outputFileName.toString()));
			
			StringBuilder line = new StringBuilder();
			
			// Write header line
			for (int i = 0; i < reprojectionErrorList.size(); i++) {
				line.append(reprojectionErrorList.get(i).getOperationType());
				line.append(",");
			}
			writer.write(line.toString());
			writer.newLine();
			
			// Write each verification point error in term of distance in 3D space
			for (int i = 0; i < verificationData.getPointLocation().size(); i++) {
				line = new StringBuilder();
				
				for(int j = 0; j < reprojectionErrorList.size(); j++) {
					line.append(reprojectionErrorList.get(j).getReprojectionErrorList()[i]);
					line.append(",");
				}
				
				writer.write(line.toString());
				writer.newLine();
			}
			writer.newLine();
			
			// Write each average
			line = new StringBuilder();
			for (int i = 0; i < reprojectionErrorList.size(); i++) {
				line.append(reprojectionErrorList.get(i).getAverageError());
				line.append(",");
			}
			writer.write(line.toString());
			writer.newLine();
			
			writer.close();
		}	
	}
	
	private static String getOutputFileName (StringBuilder dirNameBuilder, String operationType, int size) {
		StringBuilder outputFileName = new StringBuilder();
		outputFileName.append(dirNameBuilder.toString());
		outputFileName.append("/result - ");
		outputFileName.append(dataFileName);
		outputFileName.append(" - ");
		outputFileName.append(size);
		outputFileName.append(" ");
		outputFileName.append(operationType);
		outputFileName.append(".csv");
		
		return outputFileName.toString();
	}

//---------------------------------------------Calibration Part----------------------------------------------------------------
	private static Jama.Matrix calculateCalibrationMatrix (rawDataInArrayObject data, String operationType, String secondOperationType) throws NoSquareException{
		if (data != null) {
			ArrayList<double[][]> transformationMatrixList = data.getTransformationMatrix();
			ArrayList<double[][]> pointLocationList = data.getPointLocation();
			ArrayList<double[][]> screenPointLocationList = data.getScreenPointLocation();

			ArrayList<double[][]> transformedPointLocList = new ArrayList<double[][]>();

			if (transformationMatrixList.size() == pointLocationList.size()) {
				for (int i = 0; i < transformationMatrixList.size(); i++) {
					double[][] tmp = transformPointToMarkerCoordinate(transformationMatrixList.get(i),
							pointLocationList.get(i));

					if (tmp != null) {
						transformedPointLocList.add(tmp);
					} else {
						System.out.println("tmp == null");
					}
				}
			} else {
				System.out.println("transformationMatrixList.size() != pointLocationList.size()");
			}

			if (transformedPointLocList.size() == screenPointLocationList.size()) {
				SPAAM_SVD svd = new SPAAM_SVD();

				for (int i = 0; i < transformedPointLocList.size(); i++) {
					double[][] transformedPointLoc = transformedPointLocList.get(i);
					double[][] screenPoint = screenPointLocationList.get(i);

					if (screenPointCalib) {
						svd.corr_points
								.add(new Correspondence_Pair(transformedPointLoc[0][0], transformedPointLoc[1][0],
										transformedPointLoc[2][0], screenPoint[0][0] / screenPoint[2][0],
										screenPoint[1][0] / screenPoint[2][0], screenPoint[2][0]));
					} else {
						svd.corr_points.add(new Correspondence_Pair(transformedPointLoc[0][0],
								transformedPointLoc[1][0], transformedPointLoc[2][0], screenPoint[0][0],
								screenPoint[1][0], screenPoint[2][0]));
					}
				}
				
				switch (operationType) {
					case "DLT": 
						return svd.projectionDLTImpl(svd.corr_points);
					case "DLT2":
						return svd.projectionDLTImpl2(svd.corr_points);
					case "LS":
						return svd.leastSquareOperation(svd.corr_points);
					case "ES":
						return svd.openCVEstimateAffine(svd.corr_points);
					case "RS2":
						return svd.ransac2D(secondOperationType);
					default:
						return null;
				}
			}
		}
		
		return null;
	}
//---------------------------------------------End of Calibration Part----------------------------------------------------------------

// ---------------------------------------------VerificationPart----------------------------------------------------------------
	private static reprojectionErrorObject verificationOfCalibrationMatrix(Jama.Matrix calibrationMatrix, String outputFileName) throws IOException {
		
		if (verificationData != null) {
			reprojectionErrorObject reprojectionError = new reprojectionErrorObject();
			
			ArrayList<double[][]> transformationMatrixList = verificationData.getTransformationMatrix();
			ArrayList<double[][]> pointLocationList = verificationData.getPointLocation();
			ArrayList<double[][]> screenPointLocationList = verificationData.getScreenPointLocation();

			ArrayList<double[][]> transformedPointLocList = new ArrayList<double[][]>();

			if (transformationMatrixList.size() == pointLocationList.size()) {
				for (int i = 0; i < transformationMatrixList.size(); i++) {
					double[][] tmp = transformPointToMarkerCoordinate(transformationMatrixList.get(i),
							pointLocationList.get(i));

					if (tmp != null) {
						transformedPointLocList.add(tmp);
					} else {
						System.out.println("tmp == null");
					}
				}
			} else {
				System.out.println("transformationMatrixList.size() != pointLocationList.size()");
			}

			if (calibrationMatrix != null) {
				double[] reprojectionErrorList = new double[transformationMatrixList.size()];
				
				Matrix calibMatrix = new Matrix(calibrationMatrix.getArray());
				ArrayList<double[][]> returnedScreenPoint = new ArrayList<double[][]>();

				for (int i = 0; i < transformedPointLocList.size(); i++) {
					double[][] tmp = transformedPointLocList.get(i);

					if (tmp != null) {
						returnedScreenPoint.add(getHoloLensLocalTransform(tmp, calibMatrix));
					}
				}

				if (returnedScreenPoint != null && returnedScreenPoint.size() > 0) {
					BufferedWriter writer = new BufferedWriter(new FileWriter(outputFileName));

					StringBuilder matrixBuilder = new StringBuilder();
					matrixBuilder.append("{ ");
					for (int i = 0; i < calibMatrix.getNrows(); i++) {
						matrixBuilder.append("{ ");

						for (int j = 0; j < calibMatrix.getNcols(); j++) {
							matrixBuilder.append(df.format(calibMatrix.getValueAt(i, j)));

							if (j < calibMatrix.getNcols() - 1) {
								matrixBuilder.append(", ");
							}
						}

						matrixBuilder.append(" } ");

						if (i < calibMatrix.getNrows() - 1) {
							matrixBuilder.append(", ");
						}
					}
					matrixBuilder.append("}");

					writer.write(matrixBuilder.toString());
					writer.newLine();
					writer.newLine();

					int counter = 0;
					double totalD = 0.00;
					double totalX = 0.00;
					double totalY = 0.00;
					double totalZ = 0.00;

					for (int i = 0; i < returnedScreenPoint.size(); i++) {
						double[][] tmpArray = returnedScreenPoint.get(i);

						StringBuilder builder = new StringBuilder();
						for (int j = 0; j < tmpArray.length; j++) {
							builder.append(tmpArray[j][0] + "");
							if (j < tmpArray.length - 1)
								builder.append(",");
						}

						builder.append(", ,");

						double[][] screenPoint = screenPointLocationList.get(i);
						for (int j = 0; j < screenPoint.length; j++) {
							builder.append(screenPoint[j][0] + "");
							if (j < screenPoint.length - 1)
								builder.append(",");
						}

						builder.append(", ,");

						// Calculate x, y, z diff
						double x = tmpArray[0][0] - screenPoint[0][0];
						double y = tmpArray[1][0] - screenPoint[1][0];
						double z = tmpArray[2][0] - screenPoint[2][0];

						builder.append(Double.toString(x));
						builder.append(",");
						builder.append(Double.toString(y));
						builder.append(",");
						builder.append(Double.toString(z));

						builder.append(", ,");

						// Calculate distance diff using d= sqrt[(x1-x0)^2 + (y1-y0)^2 + (z1-z0)^2]
						double d = Math.sqrt(Math.pow((tmpArray[0][0] - screenPoint[0][0]), 2)
								+ Math.pow((tmpArray[1][0] - screenPoint[1][0]), 2)
								+ Math.pow((tmpArray[2][0] - screenPoint[2][0]), 2));
						builder.append(Double.toString(d));
						reprojectionErrorList[i] = d;

						writer.write(builder.toString());
						writer.newLine();

						counter++;
						totalD += Math.abs(d);
						totalX += Math.abs(x);
						totalY += Math.abs(y);
						totalZ += Math.abs(z);
					}//End of for (int i = 0; i < returnedScreenPoint.size(); i++)

					String xError = "x Error = " + Double.toString(totalX / counter);
					String yError = "y Error = " + Double.toString(totalY / counter);
					String zError = "z Error = " + Double.toString(totalZ / counter);
					String finalError = "Mean Error = " + Double.toString(totalD / counter);

					writer.newLine();
					writer.write(xError);
					writer.newLine();
					writer.write(yError);
					writer.newLine();
					writer.write(zError);
					writer.newLine();
					writer.write(finalError);
					writer.newLine();
					writer.close();
					
					reprojectionError.setReprojectionErrorList(reprojectionErrorList);
					reprojectionError.setAverageError(totalD / counter);

					return reprojectionError;
					
				}//End of if (returnedScreenPoint != null && returnedScreenPoint.size() > 0)
			} // End of if (calibrationMatrix != null)
		}
		return null;
	}
//---------------------------------------------End of Verification Part----------------------------------------------------------------

	public static double[][] getHoloLensLocalTransform(double[][] transformedPoint, Matrix calibrationMatrix) {
		Matrix transformedPointMatrix = new Matrix(transformedPoint);

		MatrixMathematics math = new MatrixMathematics();

		Matrix tmp = math.multiply(calibrationMatrix, transformedPointMatrix);

		for (int i = 0; i < 4; i++) {
			tmp.setValueAt(i, 0, Double.parseDouble(df.format(tmp.getValueAt(i, 0) * (1 / tmp.getValueAt(3, 0)))));
		}

		if (screenPointCalib) {
			tmp.setValueAt(0, 0, Double.parseDouble(df.format(tmp.getValueAt(0, 0) * (tmp.getValueAt(2, 0)))));
			tmp.setValueAt(1, 0, Double.parseDouble(df.format(tmp.getValueAt(1, 0) * (tmp.getValueAt(2, 0)))));
		}

		return tmp.getValues();
	}

	public static double[][] transformPointToMarkerCoordinate(double[][] tranformationMatrix,
			double[][] pointLocation) {

		Matrix matrix = new Matrix(tranformationMatrix);

		MatrixMathematics math = new MatrixMathematics();

		try {

			Matrix result = math.inverse(matrix);

			Matrix finalResult = math.multiply(result, new Matrix(pointLocation));

			return finalResult.getValues();

		} catch (NoSquareException e) {
			e.printStackTrace();
		}

		return null;
	}

	public static rawDataInArrayObject readCSV(String fileName, int size) {
		String csvFile = fileName;
		String line = "";
		String cvsSplitBy = ",";

		try (BufferedReader br = new BufferedReader(new FileReader(csvFile))) {

			rawDataInArrayObject calibrationData = new rawDataInArrayObject();

			ArrayList<double[][]> transformationMatrixList = new ArrayList<double[][]>();
			ArrayList<double[][]> pointLocationList = new ArrayList<double[][]>();
			ArrayList<double[][]> screenPointLocationList = new ArrayList<double[][]>();

			ArrayList<double[][]> veriTransformationMatrixList = new ArrayList<double[][]>();
			ArrayList<double[][]> veriPointLocationList = new ArrayList<double[][]>();
			ArrayList<double[][]> veriScreenPointLocationList = new ArrayList<double[][]>();

			int count = 0;
			while ((line = br.readLine()) != null) {
				double[][] transformationMatrix = new double[4][4];
				double[][] pointLocation = new double[4][1];
				double[][] screenPointLocation = new double[3][1];

				// use comma as separator
				String[] data = line.split(cvsSplitBy);

				//TODO: simplify this part instead of using hard coding
				// Assign data into correct position of HoloLens's transformation matrix, world's point location matrix and screen's point location matrix
				if (data.length == 18) {
					for (int i = 0; i < data.length; i++) {
						switch (i) {
						case 0:
							transformationMatrix[0][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 1:
							transformationMatrix[0][1] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 2:
							transformationMatrix[0][2] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 3:
							transformationMatrix[1][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 4:
							transformationMatrix[1][1] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 5:
							transformationMatrix[1][2] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 6:
							transformationMatrix[2][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 7:
							transformationMatrix[2][1] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 8:
							transformationMatrix[2][2] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 9:
							transformationMatrix[0][3] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 10:
							transformationMatrix[1][3] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 11:
							transformationMatrix[2][3] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 12:
							pointLocation[0][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 13:
							pointLocation[1][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 14:
							pointLocation[2][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 15:
							screenPointLocation[0][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						case 16:
							if (screenPointCalib) {
								screenPointLocation[1][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							} else {
								screenPointLocation[1][0] = -Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							}

							break;
						case 17:
							screenPointLocation[2][0] = Double.parseDouble(data[i].replaceAll("[^\\d.-]", ""));
							break;
						default:
							break;
						}
					}

					for (int j = 0; j < 4; j++) {
						if (j < 3) {
							transformationMatrix[3][j] = 0.00;
						} else {
							transformationMatrix[3][j] = 1.00;
						}
					}

					pointLocation[3][0] = 1.00;
				} else {
					return null;
				}

				if (count < size) {
					double[][] newPointLocation = new double[4][1];
					double[][] newScreenPointLocation = new double[3][1];

					// Original Point, these data are for calculation of calibration matrix
					transformationMatrixList.add(transformationMatrix);
					pointLocationList.add(pointLocation);
					screenPointLocationList.add(screenPointLocation);

					// Add in extra calibration points using the edge of calibration cross. Manually add in +/- value based on measurement
					if (addPoint) {
						// left
						transformationMatrixList.add(transformationMatrix);

						newPointLocation[0][0] = pointLocation[0][0] - 9.83;
						newPointLocation[1][0] = pointLocation[1][0];
						newPointLocation[2][0] = pointLocation[2][0];
						newPointLocation[3][0] = pointLocation[3][0];
						pointLocationList.add(newPointLocation);

						if (String.valueOf(count).endsWith("0")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0448;
						} else if (String.valueOf(count).endsWith("1")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0872;
						} else if (String.valueOf(count).endsWith("2")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0871;
						} else if (String.valueOf(count).endsWith("3")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0903;
						} else if (String.valueOf(count).endsWith("4")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0897;
						} else if (String.valueOf(count).endsWith("5")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0893;
						} else if (String.valueOf(count).endsWith("6")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0895;
						} else if (String.valueOf(count).endsWith("7")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0894;
						} else if (String.valueOf(count).endsWith("8")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.091;
						} else if (String.valueOf(count).endsWith("9")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] - 0.0904;
						}

						newScreenPointLocation[1][0] = screenPointLocation[1][0];
						newScreenPointLocation[2][0] = screenPointLocation[2][0];
						screenPointLocationList.add(newScreenPointLocation);

						// right
						transformationMatrixList.add(transformationMatrix);

						newPointLocation[0][0] = pointLocation[0][0] + 9.83;
						newPointLocation[1][0] = pointLocation[1][0];
						newPointLocation[2][0] = pointLocation[2][0];
						newPointLocation[3][0] = pointLocation[3][0];
						pointLocationList.add(pointLocation);

						if (String.valueOf(count).endsWith("0")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0448;
						} else if (String.valueOf(count).endsWith("1")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0872;
						} else if (String.valueOf(count).endsWith("2")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0871;
						} else if (String.valueOf(count).endsWith("3")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.093;
						} else if (String.valueOf(count).endsWith("4")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0897;
						} else if (String.valueOf(count).endsWith("5")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0893;
						} else if (String.valueOf(count).endsWith("6")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0895;
						} else if (String.valueOf(count).endsWith("7")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0894;
						} else if (String.valueOf(count).endsWith("8")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.091;
						} else if (String.valueOf(count).endsWith("9")) {
							newScreenPointLocation[0][0] = screenPointLocation[0][0] + 0.0904;
						}

						newScreenPointLocation[1][0] = screenPointLocation[1][0];
						newScreenPointLocation[2][0] = screenPointLocation[2][0];
						screenPointLocationList.add(newScreenPointLocation);

						// top
						transformationMatrixList.add(transformationMatrix);

						newPointLocation[0][0] = pointLocation[0][0];
						newPointLocation[1][0] = pointLocation[1][0] - 9.83;
						newPointLocation[2][0] = pointLocation[2][0];
						newPointLocation[3][0] = pointLocation[3][0];
						pointLocationList.add(pointLocation);

						newScreenPointLocation[0][0] = screenPointLocation[0][0];
						if (String.valueOf(count).endsWith("0")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0902;
						} else if (String.valueOf(count).endsWith("1")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0896;
						} else if (String.valueOf(count).endsWith("2")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0904;
						} else if (String.valueOf(count).endsWith("3")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0893;
						} else if (String.valueOf(count).endsWith("4")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0898;
						} else if (String.valueOf(count).endsWith("5")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0902;
						} else if (String.valueOf(count).endsWith("6")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0904;
						} else if (String.valueOf(count).endsWith("7")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0905;
						} else if (String.valueOf(count).endsWith("8")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0905;
						} else if (String.valueOf(count).endsWith("9")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] + 0.0905;
						}
						newScreenPointLocation[2][0] = screenPointLocation[2][0];
						screenPointLocationList.add(newScreenPointLocation);

						// bottom
						transformationMatrixList.add(transformationMatrix);

						newPointLocation[0][0] = pointLocation[0][0];
						newPointLocation[1][0] = pointLocation[1][0] + 9.83;
						newPointLocation[2][0] = pointLocation[2][0];
						newPointLocation[3][0] = pointLocation[3][0];
						pointLocationList.add(pointLocation);

						newScreenPointLocation[0][0] = screenPointLocation[0][0];
						if (String.valueOf(count).endsWith("0")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0902;
						} else if (String.valueOf(count).endsWith("1")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0896;
						} else if (String.valueOf(count).endsWith("2")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0904;
						} else if (String.valueOf(count).endsWith("3")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0893;
						} else if (String.valueOf(count).endsWith("4")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0898;
						} else if (String.valueOf(count).endsWith("5")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0902;
						} else if (String.valueOf(count).endsWith("6")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0904;
						} else if (String.valueOf(count).endsWith("7")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0905;
						} else if (String.valueOf(count).endsWith("8")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0905;
						} else if (String.valueOf(count).endsWith("9")) {
							newScreenPointLocation[1][0] = screenPointLocation[1][0] - 0.0905;
						}
						newScreenPointLocation[2][0] = screenPointLocation[2][0];
						screenPointLocationList.add(newScreenPointLocation);
					}
				} else if (count >= veriStartPoint) {
					//Assign the rest of the data for verification and calculation of reprojection error
					veriTransformationMatrixList.add(transformationMatrix);
					veriPointLocationList.add(pointLocation);
					veriScreenPointLocationList.add(screenPointLocation);
				}

				count++;
			} // End of while ((line = br.readLine()) != null)

			// Data for verification
			if (veriTransformationMatrixList.size() == veriPointLocationList.size() && veriPointLocationList.size() == veriScreenPointLocationList.size()) {
				verificationData.setTransformationMatrix(veriTransformationMatrixList);
				verificationData.setPointLocation(veriPointLocationList);
				verificationData.setScreenPointLocation(veriScreenPointLocationList);

				System.out.println("Done assigning verification data");
			}

			//Data for calculation
			if (transformationMatrixList.size() == pointLocationList.size() && pointLocationList.size() == screenPointLocationList.size()) {
				calibrationData.setTransformationMatrix(transformationMatrixList);
				calibrationData.setPointLocation(pointLocationList);
				calibrationData.setScreenPointLocation(screenPointLocationList);

				return calibrationData;
			}

		} catch (IOException e) {
			e.printStackTrace();
		}

		return null;
	}
}
