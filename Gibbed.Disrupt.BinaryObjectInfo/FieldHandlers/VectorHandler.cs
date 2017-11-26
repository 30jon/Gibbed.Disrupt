/* Copyright (c) 2014 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using Gibbed.Disrupt.BinaryObjectInfo.Definitions;
using Gibbed.Disrupt.FileFormats;

namespace Gibbed.Disrupt.BinaryObjectInfo.FieldHandlers
{
    internal class VectorHandler : ValueHandler<Vector>
    {
        public override byte[] Serialize(Vector value)
        {
            var data = new byte[value.points.Count * 4];
            for (int i = 0; i < value.points.Count; i++)
            {
                var point = value.points[i];
                Array.Copy(BitConverter.GetBytes(point), 0, data, i * 4, 4);
            }
            return data;
        }

        public override Vector Parse(FieldDefinition def, string text)
        {
            var parts = text.Split(',');

            var points = new List<float>();

            foreach (var part in parts)
            {
                if (!Helpers.TryParseFloat32(part, out float value))
                {
                    throw new FormatException("failed to parse Float from value " + part);
                }
                points.Add(value);
            }

            return new Vector(points);
        }

        public override Vector Deserialize(byte[] buffer, int offset, int count, out int read)
        {
            //if (!Helpers.HasLeft(buffer, offset, count, 16))
            if (!Helpers.HasLeft(buffer, offset, count, 4) || count % 4 != 0)
            {
                throw new FormatException("field type Vector requires n*4 bytes, n > 0");
            }

            read = count;
            var points = new List<float>();
            for (var i = 0; i < count / 4; i++)
            {
                points.Add(BitConverter.ToSingle(buffer, offset + i * 4));
            }
            return new Vector(points);
        }

        public override string Compose(FieldDefinition def, Vector value)
        {
            return value.ToString();
        }
    }
}
