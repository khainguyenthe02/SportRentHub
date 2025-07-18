﻿namespace SportRentHub.Entities.DTOs.ChildCourt
{
	public class ChildCourtCreateDto
	{
		public int CourtId { get; set; }
		public string ChildCourtName { get; set; }
		public string? ChildCourtDescription { get; set; }
		public string? Position { get; set; }
		public float RentCost { get; set; }
		public float? FixedRentCost { get; set; }

	}
}
