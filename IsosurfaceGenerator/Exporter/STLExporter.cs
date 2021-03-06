/*
 * This file is part of "IsosurfaceGenerator"
 *
 * Copyright (C) 2013 Keichi TAKAHASHI. All Rights Reserved.
 * Please contact Keichi Takahashi <keichi.t@me.com> for further informations.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using IsosurfaceGenerator.Utils;

namespace IsosurfaceGenerator.Exporter
{
	[Obsolete]
	public class STLExporter : IMeshExporter
	{
		private string _filename;

		private STLExporter()
		{
		}

		public STLExporter (string filename)
		{
			_filename = filename;
		}

		private unsafe void writeStruct<T>(Stream stream, T obj) where T : struct
		{
			var buf = new byte[Marshal.SizeOf(obj)];
			var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);

			Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
			handle.Free();

			stream.Write(buf, 0, buf.Length);
		}

		public void Export(List<Triangle> triangles, float isoValue)
		{
			using (var fs = File.OpenWrite(_filename)) {
				fs.Write(new byte[80], 0, 80);
				fs.Write(BitConverter.GetBytes(triangles.Count), 0, 4);

				foreach (var triangle in triangles) {
					var normal = (triangle.Vertex3 - triangle.Vertex1).Cross (triangle.Vertex2 - triangle.Vertex1).Normalize ();
					writeStruct(fs, normal);
					writeStruct(fs, triangle);
					fs.Write (new byte[2], 0, 2);
				}
			}
		}

		public void Dispose()
		{
		}
	}
}

