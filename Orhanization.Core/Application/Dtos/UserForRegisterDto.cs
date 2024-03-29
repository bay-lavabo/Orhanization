﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orhanization.Core.Application.Dtos;

public class UserForRegisterDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DepositorId { get; set; }
    public int[] UserOperationClaimIds { get; set; }

    public UserForRegisterDto()
    {
        Email = string.Empty;
        Password = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public UserForRegisterDto(string email, string password, string firstName, string lastName, string depositorId, int[] userOperationClaimIds)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        DepositorId = depositorId;
        UserOperationClaimIds = userOperationClaimIds;
    }
}
