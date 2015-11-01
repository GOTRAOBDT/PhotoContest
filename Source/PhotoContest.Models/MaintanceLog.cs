namespace PhotoContest.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class MaintanceLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
