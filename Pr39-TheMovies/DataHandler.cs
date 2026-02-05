using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace MetroTherm.Models
{
    public interface IDataHandler
    {
        IEnumerable<T> LoadData<T>() where T : class;
    }

    public class DataHandler : IDataHandler
    {
        private string DataFileName;
        
        public DataHandler(string dataFileName) => DataFileName = dataFileName; 
        
        public IEnumerable<T> LoadData<T>() where T : class
        {
            if (!File.Exists(DataFileName))
                throw new FileNotFoundException($"File not found: {DataFileName}");

            var lines = File.ReadAllLines(DataFileName);
            if (lines.Length <= 1) return new List<T>();

            var header = lines[0].Split('\t');
            var list = new List<T>();

            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split('\t');
                string GetColumn(int index) => index < columns.Length ? columns[index] : string.Empty; 

                if (typeof(T) == typeof(Customer))
                {
                    var customer = new Customer(
                        id: GetColumn(0),
                        name: GetColumn(1),
                        address: GetColumn(2),
                        equipment: null
                    );
                    list.Add(customer as T);
                }
                else if (typeof(T) == typeof(Equipment))
                {
                    var equipment = new Equipment(
                        deviceID: GetColumn(0), serialNumber: GetColumn(1),  productName: GetColumn(2),  
                        connectionstate: GetColumn(3),  currentFwVers: GetColumn(4), desiredFWVersion: GetColumn(5),  
                        paramID: GetColumn(6),   paramName: GetColumn(7), value: GetColumn(8),   
                        strVal: GetColumn(9),   paramUnit: GetColumn(10),  timestamp: GetColumn(11),  
                        catagory: GetColumn(12),  writeable: GetColumn(13),  minVal: GetColumn(14), 
                        maxVal: GetColumn(15),  stopVal: GetColumn(16), scaleVal: GetColumn(17),  
                        zoneID: GetColumn(18),  smarthomeCatagories: GetColumn(19),  enumValue: GetColumn(20)   
                    );
                    list.Add(equipment as T);
                }
                else
                {
                    throw new InvalidOperationException($"Type {typeof(T).Name} not supported");
                }
            }
            return list;
        }
    }
}
