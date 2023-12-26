﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orhanization.Core.ElasticSearch.Models;

public class SearchParameters
{
    public string IndexName { get; set; }
    public int From { get; set; } = 0;
    public int Size { get; set; } = 10;

    public SearchParameters()
    {
        IndexName = string.Empty;
    }

    public SearchParameters(string indexName, int from, int size)
    {
        IndexName = indexName;
        From = from;
        Size = size;
    }
}
