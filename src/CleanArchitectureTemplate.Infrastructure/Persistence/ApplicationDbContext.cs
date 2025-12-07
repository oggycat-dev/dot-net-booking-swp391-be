using Microsoft.EntityFrameworkCore;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Infrastructure.Persistence;

/// <summary>
/// Application database context for FPT Booking System
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<CampusChangeRequest> CampusChangeRequests => Set<CampusChangeRequest>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<FacilityType> FacilityTypes => Set<FacilityType>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<FacilityMaintenance> FacilityMaintenances => Set<FacilityMaintenance>();
    public DbSet<BookingConflict> BookingConflicts => Set<BookingConflict>();
    public DbSet<BookingHistory> BookingHistories => Set<BookingHistory>();
    public DbSet<FacilityIssueReport> FacilityIssueReports => Set<FacilityIssueReport>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ===== Campus Configuration =====
        builder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampusName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CampusCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(255);
            entity.Property(e => e.WorkingHoursStart).IsRequired();
            entity.Property(e => e.WorkingHoursEnd).IsRequired();
            
            entity.HasIndex(e => e.CampusCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            
            // Relationships
            entity.HasMany(e => e.Facilities)
                .WithOne(e => e.Campus)
                .HasForeignKey(e => e.CampusId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Users)
                .WithOne(e => e.Campus)
                .HasForeignKey(e => e.CampusId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== CampusChangeRequest Configuration =====
        builder.Entity<CampusChangeRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.ReviewComment).HasMaxLength(500);
            
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            
            // Relationships
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.CurrentCampus)
                .WithMany()
                .HasForeignKey(e => e.CurrentCampusId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.RequestedCampus)
                .WithMany()
                .HasForeignKey(e => e.RequestedCampusId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.ReviewedByAdmin)
                .WithMany()
                .HasForeignKey(e => e.ReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Holiday Configuration =====
        builder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HolidayName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.HolidayDate).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.HolidayDate);
            entity.HasIndex(e => e.IsRecurring);
        });

        // ===== FacilityType Configuration =====
        builder.Entity<FacilityType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TypeName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TypeCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DefaultDuration).IsRequired();
            entity.Property(e => e.IconUrl).HasMaxLength(255);
            
            entity.HasIndex(e => e.TypeCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            
            // Relationships
            entity.HasMany(e => e.Facilities)
                .WithOne(e => e.Type)
                .HasForeignKey(e => e.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Facility Configuration =====
        builder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FacilityCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FacilityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Building).HasMaxLength(50);
            entity.Property(e => e.Floor).HasMaxLength(20);
            entity.Property(e => e.RoomNumber).HasMaxLength(20);
            entity.Property(e => e.Capacity).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Equipment).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            
            entity.HasIndex(e => e.FacilityCode).IsUnique();
            entity.HasIndex(e => new { e.CampusId, e.TypeId, e.Status });
            entity.HasIndex(e => e.IsActive);
            
            // Relationships
            entity.HasMany(e => e.Bookings)
                .WithOne(e => e.Facility)
                .HasForeignKey(e => e.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.MaintenanceRecords)
                .WithOne(e => e.Facility)
                .HasForeignKey(e => e.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== User Configuration =====
        builder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Major).HasMaxLength(100);
            entity.Property(e => e.NoShowCount).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.BlockedReason).HasMaxLength(500);
            
            entity.HasIndex(e => e.UserCode).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsBlocked);
            
            // Relationships - Bookings as requester
            entity.HasMany(e => e.Bookings)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Relationships - Bookings as approver
            entity.HasMany(e => e.ApprovedBookings)
                .WithOne(e => e.Approver)
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== Booking Configuration =====
        builder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookingCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BookingDate).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.Property(e => e.Purpose).IsRequired().HasMaxLength(500);
            entity.Property(e => e.NumParticipants).IsRequired();
            entity.Property(e => e.EquipmentNeeded).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.LecturerEmail).HasMaxLength(100);
            entity.Property(e => e.LecturerRejectReason).HasMaxLength(500);
            entity.Property(e => e.RejectReason).HasMaxLength(500);
            entity.Property(e => e.Rating).HasPrecision(2, 1);
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            
            entity.HasIndex(e => e.BookingCode).IsUnique();
            entity.HasIndex(e => e.BookingDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.LecturerEmail);
            entity.HasIndex(e => new { e.FacilityId, e.BookingDate });
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.FacilityId, e.BookingDate, e.StartTime, e.EndTime });
            
            // Relationships - Lecturer approver
            entity.HasOne(e => e.LecturerApprover)
                .WithMany()
                .HasForeignKey(e => e.LecturerApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Relationships - Check-in performer
            entity.HasOne(e => e.CheckInPerformer)
                .WithMany()
                .HasForeignKey(e => e.CheckedInBy)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Relationships - Check-out performer
            entity.HasOne(e => e.CheckOutPerformer)
                .WithMany()
                .HasForeignKey(e => e.CheckedOutBy)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Relationships - History
            entity.HasMany(e => e.History)
                .WithOne(e => e.Booking)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Relationships - Conflicts
            entity.HasMany(e => e.ConflictsAsFirst)
                .WithOne(e => e.Booking1)
                .HasForeignKey(e => e.BookingId1)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.ConflictsAsSecond)
                .WithOne(e => e.Booking2)
                .HasForeignKey(e => e.BookingId2)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== FacilityMaintenance Configuration =====
        builder.Entity<FacilityMaintenance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            
            entity.HasIndex(e => e.FacilityId);
            entity.HasIndex(e => new { e.StartDate, e.EndDate });
            entity.HasIndex(e => e.Status);
            
            // Relationships
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Completer)
                .WithMany()
                .HasForeignKey(e => e.CompletedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== BookingConflict Configuration =====
        builder.Entity<BookingConflict>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ConflictType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.DetectedAt).IsRequired();
            entity.Property(e => e.ResolutionMethod).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.ResolutionNote).HasMaxLength(1000);
            
            entity.HasIndex(e => e.ResolvedAt);
            entity.HasIndex(e => new { e.BookingId1, e.BookingId2 });
            
            // Relationships
            entity.HasOne(e => e.Resolver)
                .WithMany()
                .HasForeignKey(e => e.ResolvedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== BookingHistory Configuration =====
        builder.Entity<BookingHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StatusFrom).HasMaxLength(50);
            entity.Property(e => e.StatusTo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ChangedAt).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            
            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.ChangedAt);
            
            // Relationships
            entity.HasOne(e => e.ChangedBy)
                .WithMany()
                .HasForeignKey(e => e.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Apply global query filters for soft delete
        builder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Campus>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Holiday>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FacilityType>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Facility>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Booking>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FacilityMaintenance>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<BookingConflict>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<BookingHistory>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FacilityIssueReport>().HasQueryFilter(e => !e.IsDeleted);

        // ===== FacilityIssueReport Configuration =====
        builder.Entity<FacilityIssueReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReportCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IssueTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IssueDescription).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Severity).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            
            entity.HasOne(e => e.Booking)
                .WithMany()
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.ReportedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReportedBy)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.NewFacility)
                .WithMany()
                .HasForeignKey(e => e.NewFacilityId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.HandledByUser)
                .WithMany()
                .HasForeignKey(e => e.HandledBy)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.ReportCode).IsUnique();
            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.ReportedBy);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // ===== Notification Configuration =====
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Body).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.MarkAsCreated(_currentUserService.UserId);
                    break;
                case EntityState.Modified:
                    entry.Entity.MarkAsModified(_currentUserService.UserId);
                    break;
            }
        }

        // Fix all DateTime properties to UTC for PostgreSQL
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue != null)
                    {
                        var dateTime = (DateTime)property.CurrentValue;
                        if (dateTime.Kind == DateTimeKind.Unspecified)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                        }
                    }
                    else if (property.Metadata.ClrType == typeof(DateTime?) && property.CurrentValue != null)
                    {
                        var dateTime = (DateTime)property.CurrentValue;
                        if (dateTime.Kind == DateTimeKind.Unspecified)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                        }
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
