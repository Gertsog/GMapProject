using GMap.NET;
using GMap.NET.WindowsForms.Markers;

namespace GMapProject
{
    public class MyMarker : GMarkerGoogle
    {
        public int Id { get; set; }
        
        public MyMarker(PointLatLng p, GMarkerGoogleType type = GMarkerGoogleType.black_small) : base(p, type)
        {
        }

        public MyMarker(int id, PointLatLng p, GMarkerGoogleType type = GMarkerGoogleType.black_small) : base(p, type)
        {
            Id = id;
        }
    }
}
