using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAProject.Models
{
    class TopUser
    {
        public int rank { get; set; }
        public string username { get; set; }
        public int score { get; set; }
        public int trueratio { get; set; }

        public TopUser (int _rank, string _username, int _score, int _trueratio)
        {
            this.rank = _rank;
            this.username = _username;
            this.score = _score;
            this.trueratio = _trueratio;
        }
    }
}
