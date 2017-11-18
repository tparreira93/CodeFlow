using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeFlow.ManualOperations
{
    public class CodeSegment
    {
        private int cursorPos;
        private int segmentStart;
        private int segmentLength;
        private string textBuffer;
        private string completeBufferSegment;
        private string simplifiedBufferSegment;
        public Boolean IsValid()
        {
            return completeBufferSegment != null && simplifiedBufferSegment != null;
        }

        private CodeSegment(string buffer, int cursorPos)
        {
            this.cursorPos = cursorPos;
            textBuffer = buffer;
            completeBufferSegment = null;
        }
        
        public int CursorPos { get => cursorPos; }
        public string TextBuffer { get => textBuffer;}
        public string CompleteTextSegment { get => completeBufferSegment;}
        public int SegmentLength { get => segmentLength; }
        public int SegmentStart { get => segmentStart; }
        public string SimplifiedBufferSegment { get => simplifiedBufferSegment; }

        public static CodeSegment ParseFromPosition(string str_begin, string str_end, string textBuffer, int cursorPos)
        {
            CodeSegment seg = new CodeSegment(textBuffer, cursorPos);
            int begin = -1, beginCode = -1;
            int length = -1;
            int end = 0;
            int platEnd = -1;

            if (textBuffer != null && !textBuffer.Equals("") && textBuffer.Length >= str_begin.Length)
            {
                beginCode = begin = textBuffer.LastIndexOf(str_begin, cursorPos, cursorPos + 1);
                end = textBuffer.IndexOf(str_end, cursorPos) + str_end.Length;
            }

            if (begin != -1 && begin <= cursorPos && end > begin)
            {
                platEnd = textBuffer.LastIndexOf(Utils.Util.NewLine, begin);
                if (platEnd > -1)
                {
                    begin = textBuffer.LastIndexOf(Utils.Util.NewLine, platEnd) + Utils.Util.NewLine.Length;
                }

                length = end - begin;
                seg.completeBufferSegment = textBuffer.Substring(begin, length);

                int dif = textBuffer.IndexOf(Utils.Util.NewLine, beginCode) - beginCode + Utils.Util.NewLine.Length;
                beginCode = textBuffer.IndexOf(Utils.Util.NewLine, beginCode) + Utils.Util.NewLine.Length;
                length -= dif;

                seg.segmentStart = beginCode;
                seg.segmentLength = textBuffer.LastIndexOf(Utils.Util.NewLine, end, Math.Abs(beginCode - length)) - beginCode;

                seg.simplifiedBufferSegment = textBuffer.Substring(seg.SegmentStart, seg.SegmentLength);
            }

            return seg;
        }
    }
}
