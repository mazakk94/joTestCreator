﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wojtasik.Interfaces
{
    public interface IAnsweredQuestion : IQuestion
    {
        List<int> ChosenAnswers { get; set; } //id
    }
}
