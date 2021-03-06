﻿using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids.Columns.Requests
{
	public class TimeColumn : GridColumnBase<RecordedRequestModel>
	{
		public TimeColumn()
			: base(r => r.Time)
		{
		}

		public override int Rank()
		{
			return 5;
		}
	}
}