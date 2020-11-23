using System;
using System.Drawing;

namespace Lab1.ModelDrawing3D
{
    public static class BresenhemLine
    {
        private static Brush Brush => new SolidBrush(Color.Black);

        public static void Draw(Graphics g, int x0, int y0, 
            int x1, int y1)
        {
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // Проверяем рост отрезка по оси икс и по оси игрек
            // Отражаем линию по диагонали, если угол наклона слишком большой
            if (steep)
            {
                Swap(ref x0, ref y0); // Перетасовка координат вынесена в отдельную функцию для красоты
                Swap(ref x1, ref y1);
            }

            // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            var dx = x1 - x0;
            var dy = Math.Abs(y1 - y0);
            var error = dx / 2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей
            var ystep = y0 < y1 ? 1 : -1; // Выбираем направление роста координаты y
            var y = y0;
            for (var x = x0; x <= x1; x++)
            {
                DrawPoint(g,steep ? y : x, steep ? x : y); // Не забываем вернуть координаты на место
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }
        
        private static void Swap(ref int x0, ref int x1)
        {
            var t = x0;
            x0 = x1;
            x1 = t;
        }
        
        private static void DrawPoint(Graphics g, int x, int y)
        {
            g.FillRectangle(Brush, x, y, 1, 1);
        }
    }
}