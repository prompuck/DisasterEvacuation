namespace Models;
public class EvacuationZone {
    public string ZoneID { get; set; }
    public LocationCoordinates LocationCoordinates { get; set; }
    public int NumberOfPeople { get; set; }
    public int UrgencyLevel { get; set; }
} 