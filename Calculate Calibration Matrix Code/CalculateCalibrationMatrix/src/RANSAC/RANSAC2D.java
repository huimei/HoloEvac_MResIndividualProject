package RANSAC;

import java.util.ArrayList;
import java.util.List;

import org.ddogleg.fitting.modelset.DistanceFromModel;
import org.ddogleg.fitting.modelset.ModelGenerator;
import org.ddogleg.fitting.modelset.ModelManager;
import org.ddogleg.fitting.modelset.ModelMatcher;
import org.ddogleg.fitting.modelset.ransac.Ransac;

import spaam.SPAAMBase.SPAAM_SVD.Correspondence_Pair;

public class RANSAC2D {
	public List<Correspondence_Pair> performRANSAC2D (List<Correspondence_Pair> corr_points) {
		List<Point2D> data = new ArrayList<Point2D> ();		
		
		for (int i = 0; i < corr_points.size(); i++) {
			Point2D point2d = new Point2D();
			
			point2d.x = corr_points.get(i).worldPoint.get(0, 0);
			point2d.y = corr_points.get(i).worldPoint.get(0, 1);
			
			data.add(point2d);
		}
		
		List<Point2D> result = perform(data);
		
		if (result != null) {
			if (result.size() >= 6) {
				List<Correspondence_Pair> inlierData = new ArrayList<Correspondence_Pair> ();
				
				for (int i = 0; i < result.size(); i++) {
					Point2D point = result.get(i);
					
					for (int j = 0; j < corr_points.size(); j++) {
						if (point.x == corr_points.get(j).worldPoint.get(0, 0) && point.y == corr_points.get(j).worldPoint.get(0, 1)) {
							
							inlierData.add(corr_points.get(j));
							break;
						}
					}
				}
				
				return inlierData;
			}else {
				System.out.println("result size < 6");
			}
		}else {
			System.out.println("result == null");
		}
		return null;
	}
	
	private static List<Point2D> perform (List<Point2D> data) {
		ModelManager<Line2D> manager = new LineManager();
		ModelGenerator<Line2D,Point2D> generator = new LineGenerator();
		DistanceFromModel<Line2D,Point2D> distance = new DistanceFromLine();

		// RANSAC or LMedS work well here
		ModelMatcher<Line2D,Point2D> alg = new Ransac<Line2D,Point2D>(234234, manager, generator, distance, 500, 0.08);

		if( !alg.process(data) ) {
			throw new RuntimeException("Robust fit failed!");
		}

		System.out.println("Match set size = " + alg.getMatchSet().size());
		
		return alg.getMatchSet();
	}
}
