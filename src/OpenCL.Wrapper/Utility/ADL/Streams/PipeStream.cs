namespace Utility.ADL.Streams
{
    /// <summary>
    ///     PipeStream is a thread-safe read/write data stream for use between two threads in a
    ///     single-producer/single-consumer type problem.
    ///     Version: 2014/12/15 1.2
    ///     Initial Version 2006/10/13 1.0 - initial version.
    ///     Update on 2008/10/9 1.1 - uses Monitor instead of Manual Reset events for more elegant synchronicity.
    ///     Update on 2014/12/15 1.2 - bugfix for read method not using offset - thanks Jörgen Sigvardsson, replace
    ///     NotImplementedExceptions with NotSupportedException
    ///     Copyright (c) 2006 James Kolpack (james dot kolpack at google mail)
    ///     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
    ///     associated documentation files (the "Software"), to deal in the Software without restriction,
    ///     including without limitation the rights to use, copy, modify, merge, publish, distribute,
    ///     sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
    ///     furnished to do so, subject to the following conditions:
    ///     The above copyright notice and this permission notice shall be included in all copies or
    ///     substantial portions of the Software.
    ///     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
    ///     INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
    ///     PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    ///     LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT
    ///     OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    ///     OTHER DEALINGS IN THE SOFTWARE.
    /// </summary>
    public class PipeStream : GenPipeStream<byte>
    {

    }
}