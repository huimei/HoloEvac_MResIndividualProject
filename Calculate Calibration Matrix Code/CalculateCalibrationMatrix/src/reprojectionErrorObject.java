
public class reprojectionErrorObject {
	private String operationType;
	
	private double[] reprojectionErrorList;
	
	private double averageError;

	public String getOperationType() {
		return operationType;
	}

	public void setOperationType(String operationType) {
		this.operationType = operationType;
	}

	public double[] getReprojectionErrorList() {
		return reprojectionErrorList;
	}

	public void setReprojectionErrorList(double[] reprojectionErrorList) {
		this.reprojectionErrorList = reprojectionErrorList;
	}

	public double getAverageError() {
		return averageError;
	}

	public void setAverageError(double averageError) {
		this.averageError = averageError;
	}
}
