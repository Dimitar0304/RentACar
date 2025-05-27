using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace RentACar.Tests.Helpers
{
    public static class EntityEntryMock
    {
        public static EntityEntry<T> Create<T>(T entity) where T : class
        {
            var mock = new Mock<EntityEntry<T>>();
            mock.Setup(x => x.Entity).Returns(entity);
            return mock.Object;
        }
    }
} 