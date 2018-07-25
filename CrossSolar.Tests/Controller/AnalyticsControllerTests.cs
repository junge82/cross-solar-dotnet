using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        private readonly AnalyticsController _analyticsController;
        
        public AnalyticsControllerTests()
        {
            var panelmockSet = new Mock<DbSet<Panel>>();

            panelmockSet.As<IAsyncEnumerable<Panel>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<Panel>(_panels.GetEnumerator()));

            panelmockSet.As<IQueryable<Panel>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Panel>(_panels.Provider));

            panelmockSet.As<IQueryable<Panel>>().Setup(m => m.Expression).Returns(_panels.Expression);
            panelmockSet.As<IQueryable<Panel>>().Setup(m => m.ElementType).Returns(_panels.ElementType);
            panelmockSet.As<IQueryable<Panel>>().Setup(m => m.GetEnumerator()).Returns(_panels.GetEnumerator());
            
            var electricitymockSet = new Mock<DbSet<OneHourElectricity>>();

            electricitymockSet.As<IAsyncEnumerable<OneHourElectricity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<OneHourElectricity>(_electricitys.GetEnumerator()));

            electricitymockSet.As<IQueryable<Panel>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<OneHourElectricity>(_electricitys.Provider));

            electricitymockSet.As<IQueryable<OneHourElectricity>>().Setup(m => m.Expression)
                .Returns(_electricitys.Expression);
            electricitymockSet.As<IQueryable<OneHourElectricity>>().Setup(m => m.ElementType)
                .Returns(_electricitys.ElementType);
            electricitymockSet.As<IQueryable<OneHourElectricity>>().Setup(m => m.GetEnumerator())
                .Returns(() => _electricitys.GetEnumerator());

            var contextOptions = new DbContextOptions<CrossSolarDbContext>();
            var mockContext = new Mock<CrossSolarDbContext>(contextOptions);
            mockContext.Setup(c => c.Set<Panel>()).Returns(panelmockSet.Object);
            mockContext.Setup(c => c.Set<OneHourElectricity>()).Returns(electricitymockSet.Object);

            var panelsRepository = new PanelRepository(mockContext.Object);
            var analyticsRepository = new AnalyticsRepository(mockContext.Object);


            _analyticsController = new AnalyticsController(analyticsRepository: analyticsRepository,
                                                           panelRepository: panelsRepository);
        }

        [Fact]
        public async Task PostTest()
        {
            var result = await _analyticsController.Post("AAAA1111BBBB2222",
               new OneHourElectricityModel() { Id = 1, DateTime = DateTime.Today, KiloWatt = 23 });

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task GetTest()
        {
            // Act

            var result = await _analyticsController.Get("AAAA1111BBBB2222");

           
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DayResultTest()
        {
            // Act

            var result = await _analyticsController.DayResults("AAAA1111BBBB2222");

            
            Assert.NotNull(result);

            var listone = ((result as OkObjectResult)?.Value as List<OneDayElectricityModel>)?[0];
            if (listone != null)
            {
                Assert.Equal(63, listone.Sum);
                Assert.Equal(21, listone.Average);
                Assert.Equal(23, listone.Maximum);
                Assert.Equal(19, listone.Minimum);
            }
            listone = ((result as OkObjectResult)?.Value as List<OneDayElectricityModel>)?[1];
            if (listone != null)
            {
                Assert.Equal(10, listone.Sum);
                Assert.Equal(10, listone.Average);
                Assert.Equal(10, listone.Maximum);
                Assert.Equal(10, listone.Minimum);
            }
        }

        private readonly IQueryable<Panel> _panels = new List<Panel>
        {
            new Panel
            {
                Id = 0,
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            },
            new Panel
            {
                Id = 1,
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            },
            new Panel
            {
                Id = 2,
                Brand = "Breva",
                Latitude = 14.345678,
                Longitude = 68.7655432,
                Serial = "AACA1111BBBB2232"
            }
        }.AsQueryable();


        private readonly IQueryable<OneHourElectricity> _electricitys = new List<OneHourElectricity>
        {
            new OneHourElectricity()
            {
                Id = 0,
                KiloWatt = 23,
                DateTime = new DateTime(2018,07,23,09,58,37,01), //2018-07-23T09:58:37+0100
                PanelId = "AAAA1111BBBB2222"
            },
            new OneHourElectricity()
            {
                Id = 1,
                KiloWatt = 21,
                DateTime = new DateTime(2018,07,23,10,58,37,01),
                PanelId = "AAAA1111BBBB2222"
            },
            new OneHourElectricity()
            {
                Id = 2,
                KiloWatt = 19,
                DateTime = new DateTime(2018,07,23,11,58,37,01),
                PanelId = "AAAA1111BBBB2222"
            },
            new OneHourElectricity()
            {
                Id = 3,
                KiloWatt = 10,
                DateTime = new DateTime(2018,07,22,11,58,37,01),
                PanelId = "AAAA1111BBBB2222"
            },
            new OneHourElectricity()
            {
                Id = 4,
                KiloWatt = 19,
                DateTime = new DateTime(2018,07,23,11,58,37,01),
                PanelId = "AACA1111BBBB2232"
            },
        }.AsQueryable();
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider innerQueryProvider)
        {
            _inner = innerQueryProvider;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {

        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        {
            
        }
        
        public TestAsyncEnumerable(Expression expression) : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
             return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public T Current => _inner.Current;

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }
    }
}

