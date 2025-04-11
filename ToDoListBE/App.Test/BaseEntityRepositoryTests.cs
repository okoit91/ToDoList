using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace App.Test
{
    public interface IDomainEntityId : IDomainEntityId<Guid> { }

    public interface IDomainEntityId<Tkey> where Tkey : IEquatable<Tkey>
    {
        Tkey Id { get; set; }
    }

    public class MockDomainEntity : IDomainEntityId<Guid>, IDomainEntityId
    {
        public Guid Id { get; set; }
    }

    public class BaseEntityRepositoryTests
    {
        private readonly DbContext _context;
        private readonly DbSet<MockDomainEntity> _dbSet;

        public BaseEntityRepositoryTests()
        {
            _context = Substitute.For<DbContext>();
            _dbSet = Substitute.For<DbSet<MockDomainEntity>>();
            _context.Set<MockDomainEntity>().Returns(_dbSet);
        }

        [Fact]
        public void DbSetAdd_ShouldCallDbSetAdd()
        {
            // Arrange
            var domainEntity = new MockDomainEntity { Id = Guid.NewGuid() };

            // Act
            _dbSet.Add(domainEntity);
            _context.SaveChanges();

            // Assert
            _dbSet.Received(1).Add(Arg.Is<MockDomainEntity>(e => e.Id == domainEntity.Id));
            _context.Received(1).SaveChanges();
        }

        [Fact]
        public void GetAll_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<MockDomainEntity>
            {
                new MockDomainEntity { Id = Guid.NewGuid() },
                new MockDomainEntity { Id = Guid.NewGuid() },
                new MockDomainEntity { Id = Guid.NewGuid() }
            };
            var mockDbSet = Substitute.For<DbSet<MockDomainEntity>, IQueryable<MockDomainEntity>>();
            ((IQueryable<MockDomainEntity>)mockDbSet).Provider.Returns(entities.AsQueryable().Provider);
            ((IQueryable<MockDomainEntity>)mockDbSet).Expression.Returns(entities.AsQueryable().Expression);
            ((IQueryable<MockDomainEntity>)mockDbSet).ElementType.Returns(entities.AsQueryable().ElementType);
            ((IQueryable<MockDomainEntity>)mockDbSet).GetEnumerator().Returns(entities.AsQueryable().GetEnumerator());

            _context.Set<MockDomainEntity>().Returns(mockDbSet);

            // Act
            var result = _context.Set<MockDomainEntity>().ToList();

            // Assert
            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities, result);
        }

        [Fact]
        public void GetById_ShouldReturnEntityById()
        {
            // Arrange
            var entity = new MockDomainEntity { Id = Guid.NewGuid() };
            _dbSet.Find(Arg.Any<Guid>()).Returns(entity);

            // Act
            var result = _context.Set<MockDomainEntity>().Find(entity.Id);

            // Assert
            Assert.Equal(entity, result);
        }
    }
}
