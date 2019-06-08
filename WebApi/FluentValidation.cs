using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Services.Models
{
    [Validator(typeof(VisitorValidator))]
    public class VisitorDto
    {        
        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public int Age { get; private set; }

        [JsonProperty]
        public int Height { get; private set; }
        
        [JsonProperty]
        public string Gender { get; private set; }

        [JsonProperty]
        public ICollection<int> TokenIds { get; private set; }
    }

    public class VisitorValidator : AbstractValidator<VisitorDto>
    {
        public VisitorValidator()
        {            
            RuleFor(x => x.Age).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Height).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Gender).NotEmpty();
            RuleFor(x => x.TokenIds).NotEmpty();

        }
    }
}
