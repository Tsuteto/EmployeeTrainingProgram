using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EmployeeTraining
{
    public class ProductWeight
    {
        public static int CalcWeight(Box box)
        {
            BoxData data = box.Data;
            if (data != null)
            {
                return CalcWeight(data);
            }
            else
            {
                return ProductWeight.Boxes[box.Size];
            }
        }
        
        public static int CalcWeight(BoxData data)
        {
            if (data.ProductID != -1)
            {
                return ProductWeight.Products[data.ProductID] * data.ProductCount + ProductWeight.Boxes[data.Size];
            }
            else
            {
                return ProductWeight.Boxes[data.Size];
            }
        }

        public static readonly Dictionary<BoxSize, int> Boxes = new Dictionary<BoxSize, int> {
            {BoxSize._8x8x8, 180},
            {BoxSize._20x10x7, 320},
            {BoxSize._22x22x8, 390}, // Actually this looks a 20x12x8 size
            {BoxSize._15x15x15, 620},
            {BoxSize._20x20x10, 620},
            {BoxSize._20x20x20, 1100},
            {BoxSize._30x20x20, 1400},
            {BoxSize._40x26x26, 2400},
        };

        public static readonly Dictionary<int, int> Products = new Dictionary<int, int> {
            {33, 650},
            {70, 680},
            {83, 2600},
            {84, 650},
            {85, 850},
            {147, 1100},
            {24, 1350},
            {39, 560},
            {55, 850},
            {62, 630},
            {66, 1050},
            {151, 650},
            {68, 1050},
            {99, 1100},
            {117, 650},
            {119, 1100},
            {128, 730},
            {132, 420},
            {67, 1050},
            {73, 1100},
            {74, 1100},
            {86, 1050},
            {115, 3800},
            {139, 1100},
            {26, 550},
            {30, 430},
            {32, 530},
            {34, 650},
            {46, 580},
            {161, 260},
            {10, 1000},
            {52, 700},
            {61, 900},
            {63, 650},
            {69, 500},
            {75, 1890},
            {36, 750},
            {37, 900},
            {64, 1050},
            {76, 450},
            {95, 500},
            {146, 900},
            {25, 330},
            {35, 600},
            {42, 2150},
            {101, 850},
            {123, 550},
            {125, 2550},
            {41, 640},
            {56, 850},
            {88, 1350},
            {96, 750},
            {120, 850},
            {141, 1100},
            {152, 350},
            {116, 680},
            {131, 360},
            {134, 680},
            {140, 700},
            {149, 1000},
            {150, 550},
            {157, 280},
            {58, 950},
            {107, 1050},
            {108, 450},
            {111, 1050},
            {114, 1050},
            {122, 450},
            {1, 1330},
            {2, 3800},
            {3, 660},
            {9, 660},
            {44, 490},
            {65, 1100},
            {77, 1550},
            {43, 490},
            {100, 2350},
            {113, 1100},
            {121, 450},
            {129, 1100},
            {133, 850},
            {145, 670},
            {11, 3700},
            {50, 800},
            {59, 1300},
            {91, 1700},
            {92, 670},
            {153, 1890},
            {102, 680},
            {104, 680},
            {105, 450},
            {106, 260},
            {112, 680},
            {127, 310},
            {164, 450},
            {57, 850},
            {135, 1100},
            {136, 1100},
            {137, 1100},
            {138, 1100},
            {144, 750},
            {103, 850},
            {109, 450},
            {110, 450},
            {124, 630},
            {126, 850},
            {158, 800},
            {6, 660},
            {7, 1330},
            {8, 3800},
            {78, 1550},
            {79, 1450},
            {82, 1450},
            {38, 545},
            {40, 670},
            {54, 400},
            {118, 850},
            {130, 1100},
            {142, 1100},
            {162, 260},
            {53, 800},
            {60, 1014},
            {93, 650},
            {97, 1700},
            {12, 700},
            {13, 700},
            {14, 700},
            {15, 700},
            {16, 700},
            {18, 700},
            {51, 530},
            {89, 2100},
            {90, 1600},
            {94, 1400},
            {27, 540},
            {28, 430},
            {45, 370},
            {49, 1050},
            {71, 1050},
            {72, 1050},
            {87, 1350},
            {98, 1700},
            {154, 1680},
            {155, 1890},
            {156, 320},
            {17, 700},
            {19, 700},
            {20, 700},
            {21, 700},
            {22, 700},
            {23, 700},
            {29, 700},
            {31, 520},
            {47, 680},
            {48, 1050},
            {148, 800},
            {163, 260},
            {4, 1380},
            {5, 3800},
            {80, 1400},
            {81, 1400},
            {159, 1450},
            {160, 1450},
            {165, 120},
            {166, 400},
            {167, 200},
            {168, 130},
            {169, 60},
            {170, 120},
            {171, 50},
            {172, 60},
            {173, 250},
            {174, 80},
            {175, 65},
            {176, 150},
            {177, 1500},
            {178, 18},
            {179, 120},
            {180, 450},
            {181, 180},
            {182, 1000},
            {183, 160},
            {184, 5000},
            {185, 1300},
            {186, 120},
            {187, 120},
            {188, 10000},
        };
    }
}