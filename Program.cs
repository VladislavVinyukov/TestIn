using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
public class Device
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Event
{
    public string DeviceId { get; set; }
    public string Key { get; set; }
    public object Value { get; set; }
    public DateTime Timestamp { get; set; }
}

public class Database
{
    public List<Device> Devices { get; set; }
    public List<Event> Events { get; set; }

    public Database()
    {
        Devices = new List<Device>();
        Events = new List<Event>();
    }
}

public class EventRequest
{
    public string DeviceId { get; set; }
}

public class Server
{
    private readonly Database database;
    private readonly ConcurrentQueue<Event> eventQueue;

    public Server()
    {
        database = new Database();
        eventQueue = new ConcurrentQueue<Event>();
    }

    public void RegisterDevice(Device device)
    {
        database.Devices.Add(device);
    }

    public void AddEvent(Event newEvent)
    {
        eventQueue.Enqueue(newEvent);
        database.Events.Add(newEvent);
    }

    public List<Event> GetEvents(EventRequest request)
    {
        return database.Events
            .Where(e => e.DeviceId == request.DeviceId)
            .OrderByDescending(e => e.Timestamp)
            .ToList();
    }
}
namespace Test2A
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            // Пример регистрации устройства
            Device device = new Device { Id = "1", Name = "Device 1" };
            server.RegisterDevice(device);

            // Пример добавления события
            Event newEvent = new Event
            {
                DeviceId = device.Id,
                Key = "temperature",
                Value = 25.5f,
                Timestamp = DateTime.Now
            };
            server.AddEvent(newEvent);

            // Пример получения списка событий для устройства
            EventRequest request = new EventRequest { DeviceId = device.Id };
            List<Event> events = server.GetEvents(request);

            foreach (Event ev in events)
            {
                Console.WriteLine($"DeviceId: {ev.DeviceId}, Key: {ev.Key}, Value: {ev.Value}, Timestamp: {ev.Timestamp}");
            }
        }
    }
}
