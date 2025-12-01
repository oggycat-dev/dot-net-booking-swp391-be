# FPT HCM Facility Booking System - Domain Model

## 📋 Entity Overview

Hệ thống bao gồm **11 entities chính** được thiết kế theo Clean Architecture và Domain-Driven Design (DDD).

## 🏗️ Entity Structure

### 1. **Campus** (Cơ sở)
- **Purpose**: Quản lý các campus của FPT (Campus 1, Campus 2)
- **Key Fields**:
  - `CampusCode`: Mã campus (HCM-C1, HCM-C2)
  - `CampusName`: Tên campus
  - `WorkingHoursStart/End`: Giờ làm việc (7:00-22:00)
- **Relationships**:
  - 1 Campus → N Facilities
  - 1 Campus → N Users

### 2. **User** (Người dùng)
- **Purpose**: Quản lý tài khoản Student, Lecturer, Admin
- **Key Fields**:
  - `UserCode`: Student ID hoặc Employee ID
  - `Email`: Email @fpt.edu.vn (bắt buộc)
  - `Role`: Student (7 days), Lecturer (30 days), Admin (unlimited)
  - `NoShowCount`: Đếm số lần không check-in
  - `IsBlocked`: Trạng thái bị khóa (3 no-shows → block 1 tháng)
- **Domain Logic**:
  - `RecordNoShow()`: Tự động block sau 3 lần
  - `GetMaxBookingDaysAhead()`: Giới hạn đặt trước theo role
  - `CanBook()`: Kiểm tra điều kiện đặt phòng

### 3. **FacilityType** (Loại phòng)
- **Purpose**: Phân loại facilities (Lab, Meeting Room, Sports, Study Room)
- **Key Fields**:
  - `TypeCode`: Mã loại (LAB, MEET, SPORT, STUDY)
  - `DefaultDuration`: Thời lượng mặc định (phút)
  - `RequiresApproval`: Có cần duyệt hay auto-approve
- **Examples**:
  - Computer Lab → 120 mins, requires approval
  - Study Room → 60 mins, auto-approve

### 4. **Facility** (Phòng/Cơ sở vật chất)
- **Purpose**: Quản lý các phòng/cơ sở vật chất có thể đặt
- **Key Fields**:
  - `FacilityCode`: Mã phòng (LAB-501, MEET-A-201)
  - `Capacity`: Sức chứa tối đa
  - `Equipment`: JSON danh sách thiết bị
  - `Status`: Available/UnderMaintenance/Unavailable
- **Relationships**:
  - N Facilities → 1 Campus
  - N Facilities → 1 FacilityType
  - 1 Facility → N Bookings

### 5. **Booking** (Đặt phòng)
- **Purpose**: Quản lý booking requests và reservations
- **Key Fields**:
  - `BookingCode`: BK-YYYYMMDD-XXXX (unique)
  - `Status`: Pending → Approved → InUse → Completed
  - `BookingDate`, `StartTime`, `EndTime`
  - `Purpose`, `NumParticipants`
- **Lifecycle**:
  ```
  Pending → Approved/Rejected
     ↓
  Confirmed (user acknowledges)
     ↓
  InUse (checked in)
     ↓
  Completed (checked out) → Can rate
     ↓
  NoShow (không check-in trong 15 phút)
  ```
- **Domain Logic**:
  - `CanCheckIn()`: Trong khoảng ±15 phút từ start time
  - `ShouldBeMarkedNoShow()`: Quá 15 phút sau start time
  - `CanCancelWithoutPenalty()`: Hơn 2 giờ trước start time
  - `OverlapsWith()`: Kiểm tra conflict với booking khác

### 6. **FacilityMaintenance** (Bảo trì)
- **Purpose**: Quản lý lịch bảo trì, ảnh hưởng đến bookings
- **Key Fields**:
  - `StartDate`, `EndDate`
  - `Status`: Scheduled → InProgress → Completed/Cancelled
  - `Reason`: Lý do bảo trì
- **Impact**: Bookings trong thời gian bảo trì sẽ bị ảnh hưởng → cần reschedule

### 7. **BookingConflict** (Xung đột đặt phòng)
- **Purpose**: Phát hiện và giải quyết conflicts
- **Conflict Types**:
  - `TimeOverlap`: Hai bookings cùng phòng, thời gian chồng lấn
  - `DoubleBooking`: Hai bookings đã approved cho cùng slot
  - `MaintenanceConflict`: Booking trung với lịch bảo trì
- **Resolution Methods**:
  - `PriorityRule`: Lecturer > Student, Earlier > Later
  - `ManualResolution`: Admin xử lý thủ công
  - `Reschedule`: Đổi thời gian/phòng
  - `Cancellation`: Hủy booking ưu tiên thấp

### 8. **BookingHistory** (Lịch sử thay đổi)
- **Purpose**: Audit trail cho mọi thay đổi status
- **Key Fields**:
  - `StatusFrom` → `StatusTo`
  - `ChangedBy`: User thực hiện thay đổi
  - `IpAddress`: IP của người thay đổi
- **Use Cases**: Compliance, debugging, user dispute resolution

### 9. **Holiday** (Ngày nghỉ)
- **Purpose**: Quản lý ngày lễ, không cho phép booking
- **Key Fields**:
  - `IsRecurring`: Lễ hàng năm (true) hay một lần (false)
  - `HolidayDate`: Ngày lễ
- **Examples**:
  - Tết Nguyên Đán (recurring)
  - Quốc khánh 2/9 (recurring)
  - Company event specific date (non-recurring)

## 🔗 Relationship Diagram

```
┌─────────────┐
│   Campus    │
└──────┬──────┘
       │ 1:N
       ├──────────────────┐
       │                  │
       ▼                  ▼
┌─────────────┐    ┌─────────────┐
│  Facility   │    │    User     │
│   Type      │    └──────┬──────┘
└──────┬──────┘           │
       │ 1:N              │ 1:N (bookings)
       │                  │ 1:N (approved)
       ▼                  │ 1:N (check-in/out)
┌─────────────┐           │
│  Facility   ├───────────┤
└──────┬──────┘           │
       │ 1:N              │
       ├──────────────┬───┤
       │              │   │
       ▼              ▼   ▼
┌─────────────┐ ┌──────────────┐
│  Booking    │ │  Facility    │
│             │ │ Maintenance  │
└──────┬──────┘ └──────────────┘
       │
       ├────────────────┬─────────────┐
       │                │             │
       ▼                ▼             ▼
┌─────────────┐  ┌──────────┐  ┌──────────┐
│  Booking    │  │ Booking  │  │ Booking  │
│  History    │  │ Conflict │  │ Conflict │
└─────────────┘  │(as B1)   │  │(as B2)   │
                 └──────────┘  └──────────┘
```

## 📊 Entity Statistics

| Entity | Properties | Navigation Props | Domain Methods | Purpose |
|--------|-----------|------------------|----------------|---------|
| Campus | 7 | 2 | 4 | Quản lý cơ sở |
| User | 18 | 2 | 11 | Quản lý người dùng |
| FacilityType | 8 | 1 | 2 | Phân loại phòng |
| Facility | 15 | 3 | 9 | Quản lý phòng |
| Booking | 26 | 7 | 20 | Đặt phòng |
| FacilityMaintenance | 10 | 3 | 7 | Bảo trì |
| BookingConflict | 10 | 3 | 4 | Xử lý xung đột |
| BookingHistory | 7 | 2 | 1 | Audit trail |
| Holiday | 5 | 0 | 1 | Ngày nghỉ |

## 🎯 Key Business Rules (Implemented in Domain)

### User Rules
1. **Email domain**: Phải @fpt.edu.vn
2. **No-show policy**: 3 lần → tự động block 1 tháng
3. **Booking limits**:
   - Student: Max 7 ngày trước, max 3 concurrent bookings
   - Lecturer: Max 30 ngày trước, unlimited concurrent
   - Admin: Unlimited

### Booking Rules
1. **Check-in window**: ±15 phút từ start time
2. **No-show threshold**: 15 phút sau start time
3. **Cancellation policy**: 
   - > 2 giờ trước: No penalty
   - < 2 giờ trước: Warning (3 warnings → block)
4. **Working hours**: 7:00 - 22:00
5. **No holidays**: Không cho phép booking vào ngày lễ

### Conflict Resolution Priority
1. Lecturer > Student
2. Earlier booking > Later booking
3. Official class > Club activity

### Maintenance Impact
- Khi schedule maintenance → tìm affected bookings
- Notify users → suggest alternatives
- Update facility status → UnderMaintenance

## 🔄 State Machines

### Booking Status Flow
```
[Created] → Pending
            ↓
        Approved ←→ Rejected
            ↓
        Confirmed
            ↓
        InUse (checked-in)
            ↓
        Completed (checked-out) → Can rate
            
Special paths:
- Pending/Approved → Cancelled (user/admin cancel)
- Approved → NoShow (không check-in trong 15 phút)
```

### Maintenance Status Flow
```
Scheduled → InProgress → Completed
    ↓
Cancelled
```

### Facility Status Flow
```
Available ←→ UnderMaintenance ←→ Unavailable
```

## 🛡️ Domain Validations

### Built-in Entity Validations
- **User**: Email format, role validation, block logic
- **Booking**: Time range validation, capacity check, overlap detection
- **Facility**: Capacity > 0, valid working hours
- **Campus**: Working hours validation (start < end)

### Cross-Entity Validations (in Application Layer)
- User can only book if not blocked and email confirmed
- Booking participants ≤ facility capacity
- Booking time within campus working hours
- No booking on holidays
- No overlapping approved bookings

## 📝 Notes for Implementation

### Next Steps
1. ✅ **Domain Entities** - COMPLETED
2. ⏭️ **DbContext Configuration** - Configure relationships, indexes
3. ⏭️ **Migrations** - Generate EF Core migrations
4. ⏭️ **Repositories** - Implement repository pattern
5. ⏭️ **Application Services** - Business logic orchestration
6. ⏭️ **DTOs & Mappings** - AutoMapper profiles
7. ⏭️ **API Controllers** - RESTful endpoints
8. ⏭️ **Authentication** - @fpt email + campus selection
9. ⏭️ **Authorization** - Role-based access control
10. ⏭️ **Background Jobs** - Auto no-show detection, conflict scanning

### Database Indexes (Important for Performance)
- `Booking`: (FacilityId, BookingDate, StartTime, EndTime)
- `Booking`: (UserId, Status)
- `User`: (Email), (UserCode)
- `Facility`: (CampusId, TypeId, Status)
- `BookingHistory`: (BookingId, ChangedAt)

### Potential Enhancements
- Add `FacilityImage` entity (multiple images per facility)
- Add `Notification` entity (in-app notifications)
- Add `RecurringBooking` entity (for lecturer semester bookings)
- Add `BookingEquipment` entity (many-to-many between Booking and Equipment)
- Add `AuditLog` entity (general system audit trail)
