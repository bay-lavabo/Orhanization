﻿using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orhanization.Core.ElasticSearch.Models;

public class ElasticSearchInsertUpdateModel : ElasticSearchModel
{
    public object Item { get; set; }

    public ElasticSearchInsertUpdateModel(object item)
    {
        Item = item;
    }

    public ElasticSearchInsertUpdateModel(Id elasticId, string indexName, object item)
        : base(elasticId, indexName)
    {
        Item = item;
    }
}
