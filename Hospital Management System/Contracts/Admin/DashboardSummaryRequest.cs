namespace Hospital_Management_System.Contracts.Admin;

public record DashboardSummaryRequest(
   
);
public record RoomOccupancyRequest();
public record TopDoctorsRequest([FromQuery(Name = "take")] int Take = 5);