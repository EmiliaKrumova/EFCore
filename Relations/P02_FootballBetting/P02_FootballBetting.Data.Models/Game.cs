﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        [ForeignKey(nameof(HomeTeam))]
        public int HomeTeamId { get; set; }
       
        public Team HomeTeam { get; set; }
        [ForeignKey(nameof(AwayTeam))]
        public int AwayTeamId { get; set; }
        [ForeignKey(nameof(AwayTeamId))]
        public Team AwayTeam { get; set; }

        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }

        public decimal HomeTeamBetRate { get; set; }
        public decimal AwayTeamBetRate { get; set; }
        public decimal DrawBetRate { get; set; }

        public DateTime DateTime { get; set; }
        public string Result { get; set; }

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; } = new List<PlayerStatistic>();
        public virtual ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();
    }
}
