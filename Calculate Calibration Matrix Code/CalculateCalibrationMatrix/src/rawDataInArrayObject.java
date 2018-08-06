import java.util.ArrayList;

public class rawDataInArrayObject {
	
	private ArrayList<double[][]> transformationMatrix;
	
	private ArrayList<double[][]> pointLocation;
	
	private ArrayList<double[][]> screenPointLocation;

	public ArrayList<double[][]> getTransformationMatrix() {
		return transformationMatrix;
	}

	public void setTransformationMatrix(ArrayList<double[][]> transformationMatrix) {
		this.transformationMatrix = transformationMatrix;
	}

	public ArrayList<double[][]> getPointLocation() {
		return pointLocation;
	}

	public void setPointLocation(ArrayList<double[][]> pointLocation) {
		this.pointLocation = pointLocation;
	}

	public ArrayList<double[][]> getScreenPointLocation() {
		return screenPointLocation;
	}

	public void setScreenPointLocation(ArrayList<double[][]> screenPointLocation) {
		this.screenPointLocation = screenPointLocation;
	}
}
