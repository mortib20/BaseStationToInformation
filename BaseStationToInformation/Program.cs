using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BaseStationToInformation;

internal abstract class Program
{
    private static bool _running = true;

    private static async Task Main(string[] args)
    {
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = false;
            _running = false;
        };
        
        using var client = new TcpClient();
        
        
        await client.ConnectAsync("192.168.2.104", 30003);

        if (!client.Connected)
        {
            throw new InvalidOperationException("Failed to connect to server.");
        }
        
        // Connected

        await using var stream = client.GetStream();
        var reader = new StreamReader(stream);
        
        while (_running)
        {
            var line = await reader.ReadLineAsync();

            await HandleLine(line);
        }
        
        Console.WriteLine("Hello, World!");
    }

    private static async Task HandleLine(string? line)
    {
        if (string.IsNullOrEmpty(line)) return;

        var strings = line.Split(',');
        var message = new BaseStationMessage
        {
            MessageType = strings[0],
            TransmissionType = Enum.Parse<TransmissionTypes>(strings[1]),
            SessionId = strings[2],
            AircraftId = strings[3],
            HexIdent = strings[4],
            FlightId = strings[5],
            MessageGenerateDate = strings[6],
            MessageGenerateTime = strings[7],
            MessageLoggedDate = strings[8],
            MessageLoggedTime = strings[9],
            Callsign = strings[10],
            Altitude = strings[11],
            GroundSpeed = strings[12],
            Track = strings[13],
            Latitude = strings[14],
            Longitude = strings[15],
            VerticalRate = strings[16],
            Squawk = strings[17],
            Alert = strings[18],
            Emergency = strings[19],
            Spi = strings[20],
            IsOnGround = strings[21]
        };

        await Task.Delay(1000);
        
        Console.WriteLine(message);
    }
}

public enum TransmissionTypes
{
    EsIdentdificationAndCategory = 1,
    EsSurfacePositionMessage = 2,
    EsAirbornePositionMessage = 3,
    EsAirborneVelocityMessage = 4,
    SurveillanceAltMessage = 5,
    SurveillanceIdMessage = 6,
    AirToAirMessage = 7,
    AllCallReply = 8    
}

internal class BaseStationMessage
{
    public string? MessageType { get; set; }
    public TransmissionTypes? TransmissionType { get; set; }
    public string? SessionId { get; set; }
    public string? AircraftId { get; set; }
    public string? HexIdent { get; set; }
    public string? FlightId { get; set; }
    public string? MessageGenerateDate { get; set; }
    public string? MessageGenerateTime { get; set; }
    public string? MessageLoggedDate { get; set; }
    public string? MessageLoggedTime { get; set; }
    
    public string? Callsign { get; set; }
    public string? Altitude { get; set; }
    public string? GroundSpeed { get; set; }
    public string? Track { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? VerticalRate { get; set; }
    public string? Squawk { get; set; }
    public string? Alert { get; set; }
    public string? Emergency { get; set; }
    public string? Spi { get; set; }
    public string? IsOnGround { get; set; }

    public DateTime GenerateDate()
    {
        return new DateTime(DateOnly.ParseExact(MessageGenerateDate, "yyyy/MM/dd", CultureInfo.InvariantCulture),
            TimeOnly.ParseExact(MessageGenerateTime, "HH:mm:ss.fff", CultureInfo.InvariantCulture));
    }
    
    public DateTime LoggedDate()
    {
        return new DateTime(DateOnly.ParseExact(MessageLoggedDate, "yyyy/MM/dd", CultureInfo.InvariantCulture),
            TimeOnly.ParseExact(MessageLoggedTime, "HH:mm:ss.fff", CultureInfo.InvariantCulture));
    }

    public override string ToString()
    {
        return
            $"{nameof(MessageType)}: {MessageType}, {nameof(TransmissionType)}: {TransmissionType}, {nameof(SessionId)}: {SessionId}, {nameof(AircraftId)}: {AircraftId}, {nameof(HexIdent)}: {HexIdent}, {nameof(FlightId)}: {FlightId}, {nameof(MessageGenerateDate)}: {MessageGenerateDate}, {nameof(MessageGenerateTime)}: {MessageGenerateTime}, {nameof(MessageLoggedDate)}: {MessageLoggedDate}, {nameof(MessageLoggedTime)}: {MessageLoggedTime}, {nameof(Callsign)}: {Callsign}, {nameof(Altitude)}: {Altitude}, {nameof(GroundSpeed)}: {GroundSpeed}, {nameof(Track)}: {Track}, {nameof(Latitude)}: {Latitude}, {nameof(Longitude)}: {Longitude}, {nameof(VerticalRate)}: {VerticalRate}, {nameof(Squawk)}: {Squawk}, {nameof(Alert)}: {Alert}, {nameof(Emergency)}: {Emergency}, {nameof(Spi)}: {Spi}, {nameof(IsOnGround)}: {IsOnGround}";
    }
}