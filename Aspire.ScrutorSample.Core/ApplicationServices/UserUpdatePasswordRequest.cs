using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;
public record UserUpdatePasswordRequest(
	string Id,
	string Password) : IRequest;
