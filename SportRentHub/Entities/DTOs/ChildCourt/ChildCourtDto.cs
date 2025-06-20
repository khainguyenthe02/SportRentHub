﻿using SportRentHub.Entities.DTOs.Court;

namespace SportRentHub.Entities.DTOs.ChildCourt
{
	public class ChildCourtDto
	{
		public int Id { get; set; }
		public int CourtId { get; set; }
		public string ChildCourtName { get; set; }
		public string ChildCourtDescription { get; set; }
		public string Position { get; set; }
		public float RentCost { get; set; }
		public float FixedRentCost { get; set; }

		public CourtDto CourtDto { get; set; }
	}
}
