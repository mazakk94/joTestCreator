using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IHistory: ITest
    {
        int Id { get; set; }
        int Score { get; set; }
        IUser User { get; set; }
        DateTime When { get; set; }
        TimeSpan Duration { get; set; }
        List<IAnsweredQuestion> ChosenAnswers { get; set; }
        int TestId { get; set; }
    }
}
