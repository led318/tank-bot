﻿/*-
 * #%L
 * Codenjoy - it's a dojo-like platform from developers to developers.
 * %%
 * Copyright (C) 2020 Codenjoy
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using API.Components;

namespace API.Misc
{
    public static class Utilities
    {
        private static readonly IDictionary<string, FieldInfo> _fieldsDictionary = new Dictionary<string, FieldInfo>();


        static Utilities()
        {
            var type = typeof(Element);

            foreach (var name in Enum.GetNames(type))
            {
                var field = type.GetField(name);
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute == null)
                    continue;

                _fieldsDictionary[attribute.Description] = field;
            }
        }

        public static string GetDescription(this Element value)
        {
            var type = value.GetType();
            string name = Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);

                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                    return attribute.Description;
            }

            return null;
        }

        public static Element GetElement(this string value)
        {
            if (!_fieldsDictionary.ContainsKey(value))
                throw new Exception("There is no such an Element constant!");

            var field = _fieldsDictionary[value];

            return (Element)field.GetValue(field.Name);
        }
    }
}
