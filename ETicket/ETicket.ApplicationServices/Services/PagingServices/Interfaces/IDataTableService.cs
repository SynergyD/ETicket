﻿using ETicket.Admin.Models.DataTables;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETicket.ApplicationServices.Services.DataTable.Interfaces
{
    public interface IDataTableService<T>
    {
        object GetDataTablePage(DataTablePagingInfo pagingInfo);
    }
}
