using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CMS.Models;

public partial class CmsproContext : DbContext
{
    public CmsproContext()
    {
    }

    public CmsproContext(DbContextOptions<CmsproContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CourseContent> CourseContents { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<FacultySubject> FacultySubjects { get; set; }

    public virtual DbSet<FeeStructure> FeeStructures { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<GroupMaster> GroupMasters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentFee> StudentFees { get; set; }

    public virtual DbSet<StudentFeesType> StudentFeesTypes { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-L93QS3JL\\SQLEXPRESS;Initial Catalog=CMSPro;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourseContent>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__course_c__0BDC8719619B8048");

            entity.ToTable("course_content");

            entity.Property(e => e.ContentId).HasColumnName("contentId");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("filePath");
            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("uploadDate");

            entity.HasOne(d => d.Subject).WithMany(p => p.CourseContents)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__course_co__subje__6477ECF3");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__departme__BE2D26B6164452B1");

            entity.ToTable("departments");

            entity.Property(e => e.DeptId).HasColumnName("deptId");
            entity.Property(e => e.DeptName)
                .HasMaxLength(50)
                .HasColumnName("deptName");
            entity.Property(e => e.HeadOfDept).HasColumnName("headOfDept");

            entity.HasOne(d => d.HeadOfDeptNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HeadOfDept)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__departmen__headO__22AA2996");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__faculty__DBBF6FB11A14E395");

            entity.ToTable("faculty");

            entity.HasIndex(e => e.Email, "UQ__faculty__AB6E61641CF15040").IsUnique();

            entity.Property(e => e.FacultyId).HasColumnName("facultyId");
            entity.Property(e => e.DeptId).HasColumnName("deptId");
            entity.Property(e => e.Doj).HasColumnName("DOJ");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Experience)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("experience");
            entity.Property(e => e.FacultyImg)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FacultyName)
                .HasMaxLength(100)
                .HasColumnName("facultyName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Qualification)
                .HasMaxLength(50)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("qualification");

            entity.HasOne(d => d.Dept).WithMany(p => p.Faculties)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__faculty__deptId__20C1E124");

            entity.HasOne(d => d.Group).WithMany(p => p.Faculties)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__faculty__groupId__21B6055D");
        });

        modelBuilder.Entity<FacultySubject>(entity =>
        {
            entity.HasKey(e => e.FacultySubjectId).HasName("PK__faculty___8A87BF3A2F10007B");

            entity.ToTable("faculty_subject");

            entity.Property(e => e.FacultySubjectId).HasColumnName("facultySubjectId");
            entity.Property(e => e.FacultyId).HasColumnName("facultyId");
            entity.Property(e => e.SemId).HasColumnName("semId");
            entity.Property(e => e.SubjectId).HasColumnName("subjectId");

            entity.HasOne(d => d.Faculty).WithMany(p => p.FacultySubjects)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__faculty_s__facul__30F848ED");

            entity.HasOne(d => d.Sem).WithMany(p => p.FacultySubjects)
                .HasForeignKey(d => d.SemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__faculty_s__semId__32E0915F");

            entity.HasOne(d => d.Subject).WithMany(p => p.FacultySubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__faculty_s__subje__31EC6D26");
        });

        modelBuilder.Entity<FeeStructure>(entity =>
        {
            entity.HasKey(e => e.FeeStructureId).HasName("PK__fee_stru__F2874CB73F466844");

            entity.ToTable("fee_structure");

            entity.Property(e => e.FeeStructureId).HasColumnName("feeStructureId");
            entity.Property(e => e.DefaultAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("defaultAmount");
            entity.Property(e => e.DeptId).HasColumnName("deptId");
            entity.Property(e => e.FeeStructureDescription)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SemId).HasColumnName("semId");

            entity.HasOne(d => d.Dept).WithMany(p => p.FeeStructures)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__fee_struc__deptI__412EB0B6");

            entity.HasOne(d => d.Sem).WithMany(p => p.FeeStructures)
                .HasForeignKey(d => d.SemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__fee_struc__semId__4222D4EF");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__feedback__2613FD246754599E");

            entity.ToTable("feedbacks");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Subject)
                .HasMaxLength(60)
                .HasColumnName("subject");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<GroupMaster>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__group_ma__88C1034D117F9D94");

            entity.ToTable("group_master");

            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0707C12930");

            entity.Property(e => e.SentDate).HasColumnType("datetime");

            entity.HasOne(d => d.Student).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Notifications_Students");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemId).HasName("PK__semester__DF18841225869641");

            entity.ToTable("semesters");

            entity.Property(e => e.SemId).HasColumnName("semId");
            entity.Property(e => e.SemName)
                .HasMaxLength(50)
                .HasColumnName("semName");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__students__4D11D63C35BCFE0A");

            entity.ToTable("students");

            entity.HasIndex(e => e.Email, "UQ__students__AB6E616438996AB5").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.Address)
                .HasMaxLength(75)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(20)
                .HasColumnName("city");
            entity.Property(e => e.CurrentSemester).HasColumnName("currentSemester");
            entity.Property(e => e.DeptId).HasColumnName("deptId");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
            entity.Property(e => e.State)
                .HasMaxLength(20)
                .HasColumnName("state");
            entity.Property(e => e.StudentImg).HasMaxLength(50);
            entity.Property(e => e.StudentName)
                .HasMaxLength(100)
                .HasColumnName("studentName");
            entity.Property(e => e.TenthPassingYear).HasColumnName("tenthPassingYear");
            entity.Property(e => e.TenthPercentage)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tenthPercentage");
            entity.Property(e => e.TenthSchool)
                .HasMaxLength(50)
                .HasColumnName("tenthSchool");
            entity.Property(e => e.Tenthmarksheet)
                .HasMaxLength(50)
                .HasColumnName("tenthmarksheet");
            entity.Property(e => e.TwelfthMarksheet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("twelfthMarksheet");
            entity.Property(e => e.TwelfthPassingYear).HasColumnName("twelfthPassingYear");
            entity.Property(e => e.TwelfthPercentage)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("twelfthPercentage");
            entity.Property(e => e.TwelfthSchool)
                .HasMaxLength(50)
                .HasColumnName("twelfthSchool");

            entity.HasOne(d => d.CurrentSemesterNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.CurrentSemester)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__students__curren__3C69FB99");

            entity.HasOne(d => d.Dept).WithMany(p => p.Students)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__students__deptId__3A81B327");

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__students__groupI__3B75D760");
        });

        modelBuilder.Entity<StudentFee>(entity =>
        {
            entity.HasKey(e => e.FeeId).HasName("PK__student___E09FF20344FF419A");

            entity.ToTable("student_fees");

            entity.Property(e => e.FeeId).HasColumnName("feeId");
            entity.Property(e => e.FeeStructureId).HasColumnName("feeStructureId");
            entity.Property(e => e.PaidAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("paidAmount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("paymentDate");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalAmount");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("transactionId");

            entity.HasOne(d => d.FeeStructure).WithMany(p => p.StudentFees)
                .HasForeignKey(d => d.FeeStructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__student_f__feeSt__4AB81AF0");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentFees)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__student_f__stude__49C3F6B7");
        });

        modelBuilder.Entity<StudentFeesType>(entity =>
        {
            entity.HasKey(e => e.FeetypeId).HasName("PK__student___3656AA5E5AEE82B9");

            entity.ToTable("student_fees_type");

            entity.Property(e => e.FeetypeId).HasColumnName("feetypeId");
            entity.Property(e => e.CollegeGroundFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("collegeGroundFee");
            entity.Property(e => e.FeeStructureId).HasColumnName("feeStructureId");
            entity.Property(e => e.InternalExam)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("internalExam");
            entity.Property(e => e.LabFees)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("labFees");
            entity.Property(e => e.TuitionFees)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("tuitionFees");

            entity.HasOne(d => d.FeeStructure).WithMany(p => p.StudentFeesTypes)
                .HasForeignKey(d => d.FeeStructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_student_fees_type_fee_structure");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__subjects__ACF9A76029572725");

            entity.ToTable("subjects");

            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.DeptId).HasColumnName("deptId");
            entity.Property(e => e.SemId).HasColumnName("semId");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .HasColumnName("subjectName");

            entity.HasOne(d => d.Dept).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__subjects__deptId__2B3F6F97");

            entity.HasOne(d => d.Sem).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.SemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__subjects__semId__2C3393D0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
