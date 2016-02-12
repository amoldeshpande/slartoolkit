/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARIntSize
    {
        public int h;

        public int w;
        public NyARIntSize()
        {
            this.w = 0;
            this.h = 0;
            return;

        }

        public NyARIntSize(int i_width, int i_height)
        {
            this.w = i_width;
            this.h = i_height;
            return;
        }

        /**
         * サイズが同一であるかを確認する。
         * 
         * @param i_width
         * @param i_height
         * @return
         * @throws NyARException
         */
        public bool isEqualSize(int i_width, int i_height)
        {
            if (i_width == this.w && i_height == this.h)
            {
                return true;
            }
            return false;
        }

        /**
         * サイズが同一であるかを確認する。
         * 
         * @param i_width
         * @param i_height
         * @return
         * @throws NyARException
         */
        public bool isEqualSize(NyARIntSize i_size)
        {
            if (i_size.w == this.w && i_size.h == this.h)
            {
                return true;
            }
            return false;

        }

    }
}