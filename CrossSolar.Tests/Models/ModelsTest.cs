using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CrossSolar.Domain;
using CrossSolar.Models;
using Xunit;

namespace CrossSolar.Tests.Models
{
    public class ModelsTest
    {

       [Fact]
        public void OneDayElectricityModelTest()
        {
            OneDayElectricityModel oneDay = new OneDayElectricityModel
            {
                Average = 2,
                DateTime = DateTime.Today,
                Maximum = 10,
                Minimum = 1,
                Sum = 22
            };

            Assert.Equal(2, oneDay.Average);
            Assert.Equal(DateTime.Today, oneDay.DateTime);
            Assert.Equal(10, oneDay.Maximum);
            Assert.Equal(1, oneDay.Minimum);
            Assert.Equal(22, oneDay.Sum);
        }

        [Fact]
        public void OneHourElectricityModelTest()
        {
            OneHourElectricityModel oneHour = new OneHourElectricityModel
            {
                Id = 1,
                KiloWatt = 23,
                DateTime = DateTime.Today
            };

            Assert.Equal(1, oneHour.Id);
            Assert.Equal(23, oneHour.KiloWatt);
            Assert.Equal(DateTime.Today, oneHour.DateTime);
        }

        [Fact]
        public void OneHourElectricityListModelTest()
        {
            OneHourElectricityListModel listModel = new OneHourElectricityListModel();
            OneHourElectricityModel oneHourElectricityModel = new OneHourElectricityModel() { Id = 1, KiloWatt = 23, DateTime = DateTime.Today };
            listModel.OneHourElectricitys = new List<OneHourElectricityModel>(){ oneHourElectricityModel };
            
            var result = listModel.OneHourElectricitys;
            IEnumerable<OneHourElectricityModel> expected = new List<OneHourElectricityModel>()
            { (new OneHourElectricityModel(){Id = 1,KiloWatt = 23,DateTime = DateTime.Today})};
            Assert.All(expected,x =>
            {
                foreach (var hourElectricityModel in result)
                {
                    Assert.Equal(1, hourElectricityModel.Id);
                    Assert.Equal(23, hourElectricityModel.KiloWatt);
                    Assert.Equal(DateTime.Today, hourElectricityModel.DateTime);
                }
            });

            Assert.Collection<OneHourElectricityModel>(result,
                elem1 => Assert.Equal(1, elem1.Id));
        }
    }
}
