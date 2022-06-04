namespace ClassJournal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Lesson
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lesson()
        {
            Attendances = new HashSet<Attendance>();
        }

        public int LessonID { get; set; }

        [Required]
        [StringLength(5)]
        public string GroupID { get; set; }

        public int TeacherID { get; set; }

        public int AudienceNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int LessonNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attendance> Attendances { get; set; }

        public virtual Audience Audience { get; set; }

        public virtual Group Group { get; set; }

        public virtual LessonHour LessonHour { get; set; }

        public virtual Teacher Teacher { get; set; }
    }
}
