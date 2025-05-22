using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toll_collection
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            try
            {
                using (IRepository<Vehicle> Vehicle = VehicleRepository.Instance)
                {
                    var Vehicle1 = new Vehicle(id: 6, "Hossain", "gec Flyover", "male", "Truck", "Yellow", "01901010203", "Ctg-111180");

                    Vehicle.Add(Vehicle1);

                    var VehicleToUpdate = Vehicle.FindById(2);

                    Vehicle.Update(VehicleToUpdate);
                    Console.WriteLine($"Vehicle {VehicleToUpdate.Id} updated successfully");
                    Console.WriteLine(VehicleToUpdate.ToString());

                    if (Vehicle.Delete(VehicleToUpdate))
                        Console.WriteLine($"Vehicle {VehicleToUpdate.Id} deleted successfully");

                    var searchResult = Vehicle.Search("APositive");
                    Console.WriteLine();
                    Console.WriteLine($"Total Vehicle found: {searchResult.Count()}");
                    Console.WriteLine("----------------------------------");

                    foreach (var d in searchResult)
                    {
                        Console.WriteLine(d.ToString());
                    }

                    var searchResultAsync = await Vehicle.SearchAsync("APositive");
                    Console.WriteLine();
                    Console.WriteLine($"Total Vehicle found asynchronously: {searchResultAsync.Count()}");
                    Console.WriteLine("----------------------------------");

                    foreach (var v in searchResultAsync)
                    {
                        Console.WriteLine(v.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }


}



public interface IModel : IDisposable
{
    int Id { get; }
    bool ValidateEntity();
}

public interface IRepository<T> : IDisposable, IEnumerable<T> where T : IModel
{
    IEnumerable<T> Data { get; }
    void Add(T entity);
    bool Delete(T entity);
    void Update(T entity);
    T FindById(int Id);
    IEnumerable<T> Search(string value);
    Task<IEnumerable<T>> SearchAsync(string value); // Async method
}

public sealed class Vehicle : IModel
{
    public int Id { get; }
    public string UserName { get; set; }
    public string Location { get; set; }
    public string Gender { get; set; }
    public string VehicleType { get; set; }
    public string VehicleColor { get; set; }
    public string ContactNumber { get; set; }
    public string LicensePlate { get; set; }


    public Vehicle(int id, string UserName, string Location, string Gender, string VehicleType, string VehicleColor,
        string contactNumber, string LicensePlate)
    {
        this.Id = id;
        this.UserName = UserName;
        this.Location = Location;
        this.Gender = Gender;
        this.VehicleType = VehicleType;
        this.VehicleColor = VehicleColor;
        this.ContactNumber = ContactNumber;
        this.LicensePlate = LicensePlate;
    }
    public bool ValidateEntity()
    {

        return true;
    }

    public override string ToString()
    {
        return $"vehicle  Info\nvehicle ID : \t{this.Id}\nLocation  : \t{this.Location}\nGender : \t{this.Gender}\nVehicleType  : \t{this.VehicleType}\nVehicleColor : \t{this.VehicleColor}\nContact No : \t{this.ContactNumber}\nLicensePlate  : \t{this.LicensePlate}";
    }

    public void Dispose()
    {


    }
}

public sealed class VehicleRepository : IRepository<Vehicle>
{
    private static VehicleRepository instance = new VehicleRepository(); // Initialize _instance here
    private List<Vehicle> data;

    public static VehicleRepository Instance
    {
        get
        {
            return instance;
        }
    }
    private VehicleRepository()
    {
        data = new List<Vehicle>
            {
                new Vehicle (1, "Md.Mohammad", "shah Amanat Bridge", "Male", "Bus", "white", "01801010101","Dhaka M-223141"),
                new Vehicle (2, "Saima", "Megna", "female",  "car", "Black", "01300000222","Ctg-444444"),
                new Vehicle (3, "Sazzad", "gec Flyover", "male",  "Truck", "Yellow","01901010203", "Ctg-111180"),
                new Vehicle (4, "Fahmida", "hanif flyover", "female",  "Motorcycle", "Red", "01372456789","Dhaka B-199593"),
            };
    }
    public void Dispose()
    {
        data.Clear();

    }
    public IEnumerable<Vehicle> Data => data;

    public Vehicle this[int index]
    {
        get
        {
            return data[index];
        }
    }

    public void Add(Vehicle entity)
    {
        if (data.Any(d => d.Id == entity.Id))
        {
            throw new Exception("Duplicate Vehicle ID, try another");
        }
        else if (entity.ValidateEntity())
        {
            data.Add(entity);
        }
        else
        {
            throw new Exception("vehicle is invalid");
        }
    }

    public bool Delete(Vehicle entity)
    {
        return data.Remove(entity);
    }

    public void Update(Vehicle entity)
    {
        int preIdx = data.FindIndex(v => v.Id == entity.Id);

        data[preIdx] = entity;
    }

    public Vehicle FindById(int Id)
    {
        var result = data.FirstOrDefault(v => v.Id == Id);
        return result;
    }


  

    
    public IEnumerable<Vehicle> Search(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return data;

        var lowerValue = value.ToLower();

        var result = from v in data.AsParallel()
                     where v.Id.ToString().Contains(lowerValue) ||
                           (v.UserName?.ToLower().StartsWith(lowerValue) ?? false) ||
                           (v.Location?.ToLower().StartsWith(lowerValue) ?? false) ||
                           (v.ContactNumber?.Contains(lowerValue) ?? false) ||
                           (v.VehicleType.ToString().ToLower().Contains(lowerValue)) ||
                           (v.VehicleColor.ToString().ToLower().Contains(lowerValue))
                     orderby v.UserName
                     select v;

        return result;
    }

  
public IEnumerator<Vehicle> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return data.GetEnumerator();
    }

    public Task<IEnumerable<Vehicle>> SearchAsync(string value)
    {
        throw new NotImplementedException();
    }
}
