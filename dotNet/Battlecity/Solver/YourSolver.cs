/*-
 * #%L
 * Codenjoy - it's a dojo-like platform from developers to developers.
 * %%
 * Copyright (C) 2018 - 2021 Codenjoy
 * %%
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public
 * License along with this program.  If not, see
 * <http://www.gnu.org/licenses/gpl-3.0.html>.
 * #L%
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Battlecity.API;

namespace Demo
{
    public class YourSolver : AbstractSolver
    {
        public YourSolver(string server) : base(server) { }

        /// <summary>
        /// Is called on each move to make decision what to do (next move)
        /// </summary>
        protected override string Get(Board board)
        {
            // Sample code

            var actions = new List<Direction>();
            var random = new Random(DateTime.UtcNow.Millisecond);
            var randomValue = random.Next(0, 3);

            actions.Add((Direction)randomValue);
            actions.Add(Direction.Act);

            string action = string.Join(",", actions);
            return action;
        }
    }
}
