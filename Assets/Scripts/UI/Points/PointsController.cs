using System;

public class PointsController : IDisposable
{
    private PointsSystem pointsSystem;
    private PointsModel pointsModel;
    private PointsView pointsView;

    public PointsController(PointsSystem pointsSystem)
    {
        this.pointsSystem = pointsSystem;
        pointsModel = new PointsModel();
        ServiceLocator.TryGet<PointsView>(out pointsView);

        pointsSystem.PointsUpdated += OnPointsUpdated;
    }

    private void OnPointsUpdated(float points)
    {
        pointsModel.SetPoints(points);
        if(pointsView == null)
            pointsView = ServiceLocator.Get<PointsView>();

        pointsView.UpdatePoints(pointsModel.PreviousPoints, pointsModel.Points);
        pointsModel.SetPreviousPoints(points);
    }

    public void Dispose()
    {
        pointsSystem.PointsUpdated -= OnPointsUpdated;
    }

    
}
