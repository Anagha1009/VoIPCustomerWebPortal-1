﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoipProjectEntities.Application.Responses;

namespace VoipProjectEntities.Application.Features.TrialBalanceCustomers.Commands.UpdateTrialBalanceCustomer
{
   public class UpdateTrailBalanceCustomerCommand : IRequest<Response<Guid>>
    {
        public Guid TrailBalanceCustomerId { get; set; }
        public DateTime Date { get; set; }
        public int TransactionType { get; set; } //enum
    }
}
