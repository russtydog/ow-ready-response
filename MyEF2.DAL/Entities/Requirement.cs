using System;

namespace MyEF2.DAL.Entities
{
    public class Requirement
    {
        public Guid Id { get; set; }
        public string? RequirementNo { get; set; }
        public string? Category { get; set; }
        public string? Question { get; set; }
        public string? Response { get; set; }
    }
}