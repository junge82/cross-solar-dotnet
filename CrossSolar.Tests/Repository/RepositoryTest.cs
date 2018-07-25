using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Repository;
using Moq;
using Xunit;

namespace CrossSolar.Tests.Repository
{
    public class RepositoryTests
    {
        [Fact]
        public void AnalyticsRepositoryTests()
        {
            var dbMock = new Mock<CrossSolarDbContext>();
            AnalyticsRepository analyticsRepository = new AnalyticsRepository(dbMock.Object);
            Assert.NotNull(analyticsRepository);
        }

        [Fact]
        public void DayAnalyticsRepositoryTests()
        {
            var dbMock = new Mock<CrossSolarDbContext>();
            DayAnalyticsRepository dayAnalyticsRepository = new DayAnalyticsRepository(dbMock.Object);
            Assert.NotNull(dayAnalyticsRepository);
        }

        [Fact]
        public void PanelRepositoryTests()
        {
            var dbMock = new Mock<CrossSolarDbContext>();
            PanelRepository panelRepository = new PanelRepository(dbMock.Object);
            Assert.NotNull(panelRepository);
        }
    }
}

