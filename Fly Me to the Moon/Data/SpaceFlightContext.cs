using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Data
{
    public class SpaceFlightContext : DbContext
    {
        public SpaceFlightContext(DbContextOptions<SpaceFlightContext> options) : base(options) { }

        public DbSet<Baggage> Baggage { get; set; } = null!;
        public DbSet<Container> Container { get; set; } = null!;
        public DbSet<ContainerFlight> ContainerFlight { get; set; } = null!;
        public DbSet<Flight> Flight { get; set; } = null!;
        public DbSet<FullHealthAnalysisResult> FullHealthAnalysisResult { get; set; } = null!;
        public DbSet<Insurance> Insurance { get; set; } = null!;
        public DbSet<Passenger> Passenger { get; set; } = null!;
        public DbSet<PassengerFlight> PassengerFlight { get; set; } = null!;
        public DbSet<Robot> Robot { get; set; } = null!;
        public DbSet<RobotModelCatalog> RobotModelCatalog { get; set; } = null!;
        public DbSet<Spaceship> Spaceship { get; set; } = null!;
        public DbSet<ServiceLog> ServiceLog { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContainerFlight>()
                .HasKey(cf => new { cf.ContainerId, cf.FlightId });

            modelBuilder.Entity<PassengerFlight>()
                .HasKey(pf => new { pf.PassengerId, pf.FlightId });

            modelBuilder.Entity<Baggage>()
                .HasOne(b => b.Passenger)
                .WithOne(p => p.Baggage)
                .HasForeignKey<Baggage>(b => b.PassengerId);

            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.Insurance)
                .WithOne(i => i.Passenger)
                .HasForeignKey<Passenger>(p => p.InsuranceId);

            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.FullHealthAnalysisResult)
                .WithOne(a => a.Passenger)
                .HasForeignKey<Passenger>(p => p.AnalysisId);

            modelBuilder.Entity<Robot>()
                .HasOne(r => r.RobotModelCatalog)
                .WithMany(rm => rm.Robot)
                .HasForeignKey(r => r.RobotModel);
            
            modelBuilder.Entity<ServiceLog>()
                .HasOne(sl => sl.Spaceship)     
                .WithMany(sh => sh.ServiceLog) 
                .HasForeignKey(sl => sl.SpaceshipName);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Spaceship)
                .WithMany(s => s.Flight)
                .HasForeignKey(f => f.SpaceshipName);

            modelBuilder.Entity<Baggage>().ToTable("baggage");
            modelBuilder.Entity<Container>().ToTable("container");
            modelBuilder.Entity<ContainerFlight>().ToTable("container_flight");
            modelBuilder.Entity<Flight>().ToTable("flight");
            modelBuilder.Entity<FullHealthAnalysisResult>().ToTable("full_health_analysis_result");
            modelBuilder.Entity<Insurance>().ToTable("insurance");
            modelBuilder.Entity<Passenger>().ToTable("passenger");
            modelBuilder.Entity<PassengerFlight>().ToTable("passenger_flight");
            modelBuilder.Entity<Robot>().ToTable("robot");
            modelBuilder.Entity<RobotModelCatalog>().ToTable("robot_model_catalog");
            modelBuilder.Entity<Spaceship>().ToTable("spaceship");
            modelBuilder.Entity<ServiceLog>().ToTable("service_log");

            base.OnModelCreating(modelBuilder);
        }
    }
}