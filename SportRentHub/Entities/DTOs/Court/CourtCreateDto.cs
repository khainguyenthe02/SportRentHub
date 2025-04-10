﻿namespace SportRentHub.Entities.DTOs.Court
{
    public class CourtCreateDto
    {
        public int UserId { get; set; }
        public string? CourtName { get; set; }
        public string? CourtDescription { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Street { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public List<string>? Images { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
		public TimeSpan? StartTime { get; set; }
		public TimeSpan? EndTime { get; set; }
	}
}
