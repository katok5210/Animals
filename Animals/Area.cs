using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Animals
{
    internal class Area
    {
        private int column = 0;
        private int row = 0;
        private int count = 0;
        private int option = 1;
        private Action<string> log;
        private int core;

        public int AnimalCount { get { return count; } }

        List<List<AreaElement>> area = new List<List<AreaElement>>();

        public Area(int row, int column, int min = 0, int max = 100, int core = 67)
        {
            this.column = column;
            this.row = row;
            MyUniform rand = new MyUniform(core);
            for (int i = 0; i < row; i++)//y
            {
                area.Add(new List<AreaElement>());
                for (int j = 0; j < column; j++)//x
                {
                    area.Last().Add(new AreaElement(j, i, min, max, rand));
                }
            }
            CalculateAround();
            this.core = core;
        }
        public void SetRandom(int core)
        {
            this.core = core;
        }
        private void CalculateAround()
        {

            for (int i = 0; i < row; i++)//y
            {
                for (int j = 0; j < column; j++)//x
                {
                    SetAround(j, i);
                }
            }
        }

        private void SetAround(int X, int Y)
        {
            List<AreaNeighbor> food = new List<AreaNeighbor>();
            for (int i = 0; i < 5; i++)
            {
                food.Add(GetAround(X, Y, i));
            }
            area[X][Y].SetFoodAround(food);
        }

        private AreaNeighbor GetAround(int centerX, int centerY, int distance)
        {
            List<AreaElement> Neighbors = new List<AreaElement>();
            List<int[]> indexes = GetIndexes(distance * 2 + 1);

            AreaNeighbor curr = new AreaNeighbor(Neighbors);
            AreaElement item;

            foreach (var i in indexes)
            {
                item = area[GetlegalY(centerY, i[0], distance)][GetlegalX(centerX, i[1], distance)];
                if (!curr.Neighbors.Contains(item))
                    if (distance <= 0 || centerX != item.X || centerY != item.Y)
                        curr.Neighbors.Add(item);
            }
            curr.CalculateFood();

            return curr;
        }

        private List<int[]> GetIndexes(int distance)
        {
            List<int[]> curr = new List<int[]>();
            if (distance == 0)
                curr.Add(new int[] { 0, 0 });
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    if (i == 0 || i == distance - 1 || j == 0 || j == distance - 1)
                    {
                        curr.Add(new int[] { i, j });
                    }
                }
            }
            return curr;
        }

        private int GetlegalX(int center, int cur, int radius)
        {
            int var = center - radius + cur;
            if (var < 0) var = 0;
            if (var >= column) var = column - 1;
            return var;
        }

        private int GetlegalY(int center, int cur, int radius)
        {
            int var = center - radius + cur;
            if (var < 0) var = 0;
            if (var >= row) var = row - 1;
            return var;
        }

        private void AddRequirment(AnimalProg animal)
        {
            animal.AddRequirment(new EatRequirement(animal));
            switch (option)
            {
                case 1:
                    animal.AddRequirment(new FirstReproductionRequirement(animal));
                    break;
                default:
                    animal.AddRequirment(new SecondReproductionRequirement(animal));
                    break;
            }
        }

        public void InitAnimal(int count, int option = 1, Action<string> log = null)
        {
            this.log = log;
            this.option = option;
            MyUniform rand = new MyUniform(core);
            MyNorm myNorm = new MyNorm(core);
            this.count = count;
            int crow, ccolumn;
            while (count > 0)
            {
                crow = (int)rand.Next(1, row - 1);
                ccolumn = (int)rand.Next(1, column - 1);
                AnimalProg animal = new AnimalProg(area[crow][ccolumn], (int)myNorm.Next(65, 30), (int)myNorm.Next(3, 2), (int)myNorm.Next(25, 5), (int)myNorm.Next(65, 45), (int)myNorm.Next(17, 13), log);
                AddRequirment(animal);
                area[crow][ccolumn].Animals.Add(animal);
                count--;
            }
        }

        private void ResetCell()
        {
            for (int i = 0; i < row; i++)//y
            {
                for (int j = 0; j < column; j++)//x
                {
                    area[j][i].ResetFood();
                }
            }
        }

        public void NexLive()
        {
            ResetCell();
            List<AnimalProg> DeadAnimals = new List<AnimalProg>();
            for (int i = 0; i < row; i++)//y
            {
                for (int j = 0; j < column; j++)//x
                {
                    DeadAnimals.Clear();
                    foreach (var animal in area[i][j].Animals.OrderBy(x => x.Age))
                    {
                        area[i][j].UpdateFoodAround();
                        if (!animal.LiveTurn())
                        {
                            DeadAnimals.Add(animal);
                            animal.isReproduce = true;
                        }
                        else
                        {
                            if (animal.CheckRequirment("ReproductionRequirement"))
                            {
                                if (!animal.isReproduce)
                                {
                                    AnimalProg partner = area[i][j].GetToReproduc();
                                    if (partner != null)
                                    {
                                        partner.isReproduce = true;
                                        animal.isReproduce = true;

                                        AnimalProg child = Reproduce(partner, animal);
                                        AddRequirment(child);
                                        area[i][j].Animals.Add(child);
                                        if (log != null)
                                            log(string.Format("The animal {0} and {1} is now parents they have been born {2}", animal.Status, partner.Status, child.Status));
                                        count++;
                                    }
                                }
                            }
                            animal.isReproduce = false;
                        }
                    }
                    foreach (var animal in DeadAnimals)
                    {
                        area[i][j].Animals.Remove(animal);
                        count--;
                    }
                }
            }
        }
        public AnimalProg Reproduce(AnimalProg perentFirst, AnimalProg perentSecond)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int Energy;
            int Distance;
            int Eat;
            double Helth;
            if (random.Next(0, 100) > 50)
            {
                if (random.Next(0, 100) > 50)
                {
                    Energy = perentFirst.Energy;
                    Distance = perentSecond.Distance;
                    Eat = perentFirst.Eat;
                    Helth = perentSecond.Helth;
                }
                else
                {
                    Energy = perentSecond.Energy;
                    Distance = perentFirst.Distance;
                    Eat = perentSecond.Eat;
                    Helth = perentFirst.Helth;
                }
            }
            else
            {
                if (random.Next(0, 100) > 50)
                {
                    Energy = perentFirst.Energy;
                    Distance = perentFirst.Distance;
                    Eat = perentSecond.Eat;
                    Helth = perentSecond.Helth;
                }
                else
                {
                    Energy = perentSecond.Energy;
                    Distance = perentSecond.Distance;
                    Eat = perentFirst.Eat;
                    Helth = perentFirst.Helth;
                }
            }
            AreaElement Place = random.Next(0, 100) > 50 ? perentFirst.Place : perentSecond.Place;

            AnimalProg child = new AnimalProg(Place, Energy, Distance, Eat, Helth, log: log);
            return child;
        }

        public void PrintArea(Action<string> action)
        {
            string outstr = string.Format("Space {0} by {1} whith {2} animal", row, column, count);

            action(outstr);
        }

        public void ClearAnimal()
        {

            for (int i = 0; i < row; i++)//y
            {
                for (int j = 0; j < column; j++)//x
                {
                    area[i][j].Animals.Clear();
                }
            }
        }

        public void PrintFullArea(Action<string> action)
        {
            PrintArea(action);


            for (int i = 0; i < row; i++)//y
            {
                for (int j = 0; j < column; j++)//x
                {
                    action(area[i][j].Status);
                }
            }
        }
    }

    class AreaElement
    {
        private int food = 0;
        private int FoodStatic = 0;

        public int X { get; }
        public int Y { get; }
        public int Food { get { return food; } }
        public List<AreaNeighbor> CellAround { get; } = new List<AreaNeighbor>();

        public List<AnimalProg> Animals { get; } = new List<AnimalProg>();

        public string Status
        {
            get
            {
                StringBuilder outstr = new StringBuilder();

                outstr.Append(String.Format("place ({0}, {1}) Food = {2}\n\r", this.X, this.Y, FoodStatic));
                if (Animals.Count > 0)
                {
                    outstr.Append("Animals: ");
                    foreach (var animal in Animals)
                    {
                        outstr.Append($"{animal.Status}, ");
                    }
                }

                
                return outstr.ToString().Remove(outstr.Length - 1);
            }
        }

        public AreaElement(int X, int Y, int min = 0, int max = 100, MyUniform rand = null)
        {
            this.X = X;
            this.Y = Y;
            if (rand != null) food = (int)rand.Next(min, max);
            else food = new Random().Next(min, max);
            FoodStatic = food;
        }

        public void ResetFood()
        {
            food = FoodStatic;
        }
        public void SetFoodAround(List<AreaNeighbor> FoodAround)
        {
            for (int i = 0; i < FoodAround.Count; i++)
            {
                this.CellAround.Add(FoodAround[i]);
            }
        }
        public int GetFoodAround(int distance)
        {
            int curr = 0;
            for (int i = 0; i < distance; i++)
            {
                curr += CellAround[i].Food;
            }
            return curr;
        }
        public void UpdateFoodAround()
        {
            foreach (var neighbors in CellAround)
            {
                neighbors.CalculateFood();
            }
        }
        public void ReduceFoodAround(int Food)
        {
            int curr = Food;
            int turncurr = Food;
            int count = 0;
            while (curr > 0)
            {
                foreach (var neighbors in CellAround)
                {
                    if (neighbors.Food > 0)
                        foreach (var element in neighbors.Neighbors.OrderByDescending((x) => x.Food))
                        {
                            if (element.Food > 0)
                            {
                                turncurr = curr - element.Food;
                                if (turncurr < 0)
                                {
                                    element.ReduceFood(curr);
                                    neighbors.ReduceFood(curr);
                                    return;
                                }
                                curr = turncurr;
                                turncurr = element.Food;
                                element.ReduceFood(turncurr);
                                neighbors.ReduceFood(turncurr);
                            }
                        }
                }
                count++;
                if (count > 10)
                    throw new Exception();
            }
        }
        public void ReduceFood(int Food)
        {
            if (Food > 0 && Food <= this.FoodStatic)
                if (!(food - Food < 0))
                    food -= Food;
                else
                {
                    throw new Exception();
                }
        }
        public AnimalProg GetToReproduc()
        {
            AnimalProg search = null;
            foreach (var element in CellAround)
            {
                search = element.GetToReproduc();
                if (search != null) return search;
            }
            return null;
        }
    }
    class AreaNeighbor
    {
        private int food = 0;
        public int Food { get { return food; } }
        public List<AreaElement> Neighbors { get; }

        public AreaNeighbor(List<AreaElement> Neighbors)
        {
            this.Neighbors = Neighbors;
        }

        public void CalculateFood()
        {
            food = 0;
            foreach (var element in Neighbors)
            {
                food += element.Food;
            }
        }
        public void ReduceFood(int Eat)
        {
            if (this.food - Eat >= 0)
            {
                this.food -= Eat;
            }
            else
            {
                throw new Exception();
            }
        }
        public AnimalProg GetToReproduc()
        {
            foreach (var element in Neighbors)
            {
                foreach (var animal in element.Animals)
                {
                    if (animal.CheckRequirment("ReproductionRequirement"))
                    {
                        if (!animal.isReproduce)
                        {
                            return animal;
                        }
                    }
                }
            }
            return null;
        }
    }

    internal abstract class Requirement
    {
        protected AnimalProg Perent = null;
        protected bool istrue = false;

        public int Level { get; protected set; }
        public bool isTrue { get { return istrue; } }
        public string Status
        {
            get
            {
                string outstr = string.Empty;

                outstr = string.Format("Requirement: {0} - {1}", this.ToString(), isTrue ? "satisfied" : "not satisfied");

                return outstr;
            }
        }
        public abstract bool Check();

    }

    internal class EatRequirement : Requirement
    {
        private int prevEnergy = 0;

        public EatRequirement()
        {
            Level = 1;
        }
        public EatRequirement(AnimalProg animal)
        {
            this.Perent = animal;
            prevEnergy = animal.Energy;
            Level = 1;
        }
        public override bool Check()
        {
            if (Perent.Eat < Perent.Energy)
            {
                if (Perent.Energy < prevEnergy)
                {
                    istrue = false;
                }
                else
                {
                    istrue = true;
                }
            }
            else
            {
                istrue = false;
            }
            return istrue;
        }
        public override string ToString()
        {
            return "EatRequirement";
        }
    }

    internal class FirstReproductionRequirement : Requirement
    {
        public FirstReproductionRequirement()
        {
            Level = 2;
        }
        public FirstReproductionRequirement(AnimalProg animal)
        {
            this.Perent = animal;
            Level = 2;
        }

        public override bool Check()
        {
            if (Perent.Age > 8)
            {
                if (Perent.Age < 50)
                {
                    Perent.CheckRequirment(1);
                    if (Perent.FoodAround > Perent.Eat) istrue = true;
                    else istrue = false;
                }
                else
                {
                    istrue = false;
                }
            }
            else
            {
                istrue = false;
            }

            return istrue;
        }

        public override string ToString()
        {
            return "ReproductionRequirement";
        }
    }
    internal class SecondReproductionRequirement : Requirement
    {

        public SecondReproductionRequirement()
        {
            Level = 1;
        }
        public SecondReproductionRequirement(AnimalProg animal)
        {
            this.Perent = animal;
            Level = 1;
        }

        public override bool Check()
        {

            if (Perent.Age > 8)
            {
                if (Perent.Age < 50)
                {
                    istrue = true;
                }
                else
                {
                    istrue = false;
                }
            }
            else
            {
                istrue = false;
            }

            return istrue;
        }
        public override string ToString()
        {
            return "ReproductionRequirement";
        }
    }
    class AnimalProg
    {
        private List<Requirement> Requirements = new List<Requirement>();
        private int energy = 0;
        private double helth = 0;
        private Action<string> log;


        public string Name { get; }
        public AreaElement Place { get; }
        public int FoodAround { get { return Place.GetFoodAround(Distance); } }
        public int Age { get; private set; }
        public int Energy { get { return energy; } }
        public int Distance { get; }
        public int Eat { get; }
        public double Helth { get; }

        public string Status
        {
            get
            {
                string outstr = string.Empty;

                outstr = string.Format("[Name: {0}, Age: {1}, Energy: {2}, Eat: {3}, Distance: {4}]", this.Name, this.Age, this.Energy, this.Eat, this.Distance);

                return outstr;
            }
        }

        public bool isReproduce = false;

        public AnimalProg()
        {
            Age = 0;
            this.energy = 25;
            this.Distance = 1;
            this.Eat = 50;
        }

        public AnimalProg(AreaElement Place, int Energy, int Distance, int Eat, double Helth, int Age = 0, Action<string> log = null)
        {
            this.Age = Age;
            this.energy = Energy;
            this.Distance = Distance;
            this.Eat = Eat;
            this.Helth = Helth;
            this.helth = Helth;
            this.Place = Place;
            this.log = log;
            Name = NameGenerator.GetName();
        }

        public void AddRequirment(Requirement requirement)
        {
            if (!Requirements.Contains(requirement))
                this.Requirements.Add(requirement);
        }

        public bool CheckRequirment(string requirementName)
        {
            var list = Requirements.Where(x => x.ToString() == requirementName);
            if (list.Count() > 0)
                return Requirements.First().isTrue;

            throw new Exception("Not requirement");
        }

        public bool CheckRequirment(int level)
        {
            var list = Requirements.Where(x => x.Level < level);
            if (list.Count() > 0)
                foreach (var item in list)
                {
                    if (!item.isTrue)
                    {
                        return false;
                    }
                }
            return true;

            throw new Exception("Not requirement");
        }
        public void UpdateRequirment()
        {
            foreach (var item in this.Requirements)
            {
                item.Check();
                if (log != null)
                    log(string.Format("Requirement  {0} update(he is {1})", item.ToString(), item.isTrue));
            }
        }

        public override string ToString()
        {
            return string.Format("simple animal");
        }

        public bool LiveTurn()
        {

            if (log != null)
                log(string.Format("{0} life plays", this.Status));
            Place.UpdateFoodAround();
            int food = Place.GetFoodAround(Distance);
            Random random = new Random();
            if (random.Next() % 100 > helth)
            {
                if (log != null)
                    log(string.Format("The animal died. disease"));
                return false;
            }
            if (food - Eat > 0)
            {
                try
                {
                    Place.ReduceFoodAround(Eat);
                }
                catch
                {
                    if (log != null)
                        log(string.Format("The animal died. malnutrition. Error"));
                    return false;
                }
                energy += (int)(Eat * 0.25);
            }
            else
            {
                if (Energy + food - Eat > 0)
                {
                    try
                    {
                        Place.ReduceFoodAround(food);
                    }
                    catch
                    {
                        if (log != null)
                            log(string.Format("The animal died. malnutrition. Error"));
                        return false;
                    }
                    energy += food - Eat;
                }
                else
                {
                    if (log != null)
                        log(string.Format("The animal died. malnutrition"));
                    return false;
                }
            }

            Age++;
            helth -= Age * new Random((int)DateTime.Now.Ticks).Next(0, 100) > 65 ? 0.045 : 0.1;
            if (log != null)
                log(string.Format("The animal life"));
            UpdateRequirment();
            return true;
        }

    }

    class AreaElementComparer : IEqualityComparer<AreaElement>
    {
        public bool Equals(AreaElement A1, AreaElement A2)
        {
            if (ReferenceEquals(A1, A2))
                return true;

            if (A2 is null || A1 is null)
                return false;

            return A1.X == A2.X
                && A1.Y == A2.Y;
        }

        public int GetHashCode(AreaElement AE) => (AE.X + 1) ^ (AE.Y + 1) * (AE.Y + AE.X + 1);
    }

    static class NameGenerator
    {

        private static string[] names = { "Аарон", "Аба", "Аббас", "Абдаль-Узза", "Абдуллах", "Абид", "Аботур", "Аввакум", "Август", "Авдей", "Авель", "Аверкий", "Авигдор", "Авирмэд", "Авксентий", "Авл", "Авнер", "Аврелий", "Автандил", "Автоном", "Агапит", "Агафангел", "Агафодор", "Агафон", "Аги", "Агриппа", "Адам", "Адар", "Адиль", "Адольф", "Адонирам", "Адриан", "Азамат", "Азарий", "Азат", "Азиз", "Азим", "Айварс", "Айдар", "Айрат", "Акакий", "Аквилий", "Акиф", "Акоп", "Аксель", "Алан", "Аланус", "Алек", "Александр", "Алексей", "Алемдар", "Алик", "Алим", "Алипий", "Алишер", "Алмат", "Алоиз", "Алон", "Альберик", "Альберт", "Альбин", "Альваро", "Альвиан", "Альвизе", "Альфонс", "Альфред", "Амадис", "Амвросий", "Амедей", "Амин", "Амир", "Амр", "Амфилохий", "Анания", "Анас", "Анастасий", "Анатолий", "Ангеляр", "Андокид", "Андрей", "Андроник", "Аннерс", "Анри", "Ансельм", "Антипа", "Антон", "Антоний", "Антонин", "Антуан", "Арам", "Арефа", "Арзуман", "Аристарх", "Аристон", "Ариф", "Аркадий", "Арсений", "Артём", "Артур", "Арфаксад", "Асаф", "Атанасий", "Атом", "Аттик", "Афанасий", "Афинагор", "Афиней", "Афиф", "Африкан", "Ахилл", "Ахмад", "Ахтям", "Ашот", "Бадар", "Барни", "Бартоломео", "Басир", "Бахтияр", "Баян", "Безсон", "Бен", "Беньямин", "Берт", "Бехруз", "Билял", "Богдан", "Болеслав", "Бонавентура", "Борис", "Борислав", "Боян", "Бронислав", "Брячислав", "Бурхан", "Бутрос", "Бямбасурэн", "Вадим", "Валентин", "Валентино", "Валерий", "Валерьян", "Вальдемар", "Вангьял", "Варлам", "Варнава", "Варфоломей", "Василий", "Вахтанг", "Велвел", "Венансио", "Венедикт", "Вениамин", "Венцеслав", "Вигго", "Викентий", "Виктор", "Викторин", "Вильгельм", "Винцас", "Виссарион", "Виталий", "Витаутас", "Вито", "Владимир", "Владислав", "Владлен", "Влас", "Воислав", "Володарь", "Вольфганг", "Вописк", "Всеволод", "Всеслав", "Вук", "Вукол", "Вышеслав", "Вячеслав", "Габриеле", "Гавриил", "Гай", "Галактион", "Галымжан", "Гамлет", "Гаспар", "Гафур", "Гвидо", "Гейдар", "Геласий", "Гелий", "Гельмут", "Геннадий", "Генри", "Генрих", "Георге", "Георгий", "Гераклид", "Герасим", "Герберт", "Герман", "Германн", "Геронтий", "Герхард", "Гийом", "Гильем", "Гинкмар", "Глеб", "Гней", "Гоар", "Горацио", "Гордей", "Градислав", "Григорий", "Гримоальд", "Гуго", "Гурий", "Густав", "Гьялцен", "Давид", "Дамдинсурэн", "Дамир", "Даниил", "Дарий", "Демид", "Демьян", "Денеш", "Денис", "Децим", "Джаббар", "Джамиль", "Джан", "Джанер", "Джанфранко", "Джафар", "Джейкоб", "Джихангир", "Джованни", "Джон", "Джохар", "Джулиано", "Джулиус", "Дино", "Диодор", "Дитер", "Дитмар", "Дитрих", "Дмитрий", "Доминик", "Дональд", "Донат", "Дорофей", "Досифей", "Евгений", "Евграф", "Евдоким", "Еврит", "Евсей", "Евстафий", "Евтихан", "Евтихий", "Егор", "Елеазар", "Елисей", "Емельян", "Епифаний", "Ербол", "Ерванд", "Еремей", "Ермак", "Ермолай", "Ерофей", "Ефим", "Ефрем", "Жан", "Ждан", "Жером", "Жоан", "Захар", "Захария", "Збигнев", "Зденек", "Зейналабдин", "Зенон", "Зеэв", "Зигмунд", "Зинон", "Зия", "Золтан", "Зосима", "Иакинф", "Иан", "Ибрагим", "Ибрахим", "Иван", "Игнатий", "Игорь", "Иероним", "Иерофей", "Израиль", "Икрима", "Иларий", "Илия", "Илларион", "Илмари", "Ильфат", "Илья", "Имран", "Иннокентий", "Иоаким", "Иоанн", "Иоанникий", "Иоахим", "Иов", "Иоганн", "Иоганнес", "Ионафан", "Иосафат", "Ираклий", "Иржи", "Иринарх", "Ириней", "Иродион", "Иса", "Исаак", "Исаакий", "Исаия", "Исидор", "Ислам", "Исмаил", "Истислав", "Истома", "Истукарий", "Иштван", "Йюрген", "Кадваллон", "Кадир", "Казимир", "Каликст", "Калин", "Каллистрат", "Кальман", "Канат", "Карен", "Карлос", "Карп", "Картерий", "Кассиан", "Кассий", "Касторий", "Касьян", "Катберт", "Квинт", "Кехлер", "Киллиан", "Ким", "Кир", "Кириак", "Кирилл", "Клаас", "Клавдиан", "Клеоник", "Климент", "Кондрат", "Конон", "Конрад", "Константин", "Корнелиус", "Корнилий", "Коррадо", "Косьма", "Кратет", "Кратипп", "Крис", "Криспин", "Кристиан", "Кронид", "Кузьма", "Куприян", "Курбан", "Курт", "Кутлуг-Буга", "Кэлин", "Лаврентий", "Лавс", "Ладислав", "Лазарь", "Лайл", "Лампрехт", "Ландульф", "Лев", "Леви", "Ленни", "Леонид", "Леонтий", "Леонхард", "Лиам", "Линкей", "Логгин", "Лоренц", "Лоренцо", "Луи", "Луитпольд", "Лука", "Лукас", "Лукий", "Лукьян", "Луций", "Людовик", "Люцифер", "Макар", "Максим", "Максимиан", "Максимилиан", "Малик", "Малх", "Мамбет", "Маний", "Мануил", "Мануэль", "Мариан", "Мариус", "Марк", "Маркел", "Мартын", "Марчелло", "Матвей", "Матео", "Матиас", "Матфей", "Матфий", "Махмуд", "Меир", "Мелентий", "Мелитон", "Менахем-Мендель", "Месроп", "Мефодий", "Мечислав", "Мика", "Микеланджело", "Микулаш", "Милорад", "Мина", "Мирко", "Мирон", "Мирослав", "Митрофан", "Михаил", "Михей", "Младан", "Модест", "Моисей", "Мордехай", "Мстислав", "Мурад", "Мухаммед", "Мэдисон", "Мэлор", "Мэлс", "Назар", "Наиль", "Насиф", "Натан", "Натаниэль", "Наум", "Нафанаил", "Нацагдорж", "Нестор", "Никандр", "Никанор", "Никита", "Никифор", "Никодим", "Николай", "Нил", "Нильс", "Ноа", "Ной", "Норд", "Нуржан", "Нурлан", "Овадья", "Оге", "Одинец", "Октав", "Октавиан", "Октавий", "Октавио", "Олаф", "Оле", "Олег", "Оливер", "Ольгерд", "Онисим", "Орест", "Осип", "Оскар", "Осман", "Отто", "Оттон", "Очирбат", "Пабло", "Павел", "Павлин", "Павсикакий", "Паисий", "Палладий", "Панкратий", "Пантелеймон", "Папа", "Паруйр", "Парфений", "Патрик", "Пафнутий", "Пахомий", "Педро", "Пётр", "Пимен", "Пинхас", "Пипин", "Питирим", "Пол", "Полидор", "Полиевкт", "Поликарп", "Поликрат", "Порфирий", "Потап", "Предраг", "Премысл", "Приск", "Прокл", "Прокопий", "Прокул", "Протасий", "Прохор", "Публий", "Рагнар", "Рагуил", "Радмир", "Радослав", "Разумник", "Раймонд", "Рамадан", "Рамазан", "Рахман", "Рашад", "Рейнхард", "Ренат", "Реститут", "Ричард", "Роберт", "Родерик", "Родион", "Рожер", "Розарио", "Роман", "Ромен", "Рон", "Ронан", "Ростислав", "Рудольф", "Руслан", "Руф", "Руфин", "Рушан", "Сабит", "Савва", "Савватий", "Савелий", "Савин", "Саддам", "Садик", "Саид", "Салават", "Салих", "Саллюстий", "Салман", "Самуил", "Сармат", "Святослав", "Севастьян", "Северин", "Секст", "Секунд", "Семён", "Септимий", "Серапион", "Сергей", "Серж", "Сигеберт", "Сильвестр", "Симеон", "Симон", "Созон", "Соломон", "Сонам", "Софрон", "Спиридон", "Срджан", "Станислав", "Степан", "Стефано", "Стивен", "Таврион", "Тавус", "Тадеуш", "Тарас", "Тарасий", "Тейс", "Тендзин", "Теофил", "Терентий", "Терри", "Тиберий", "Тигран", "Тимофей", "Тимур", "Тихомир", "Тихон", "Томас", "Томоми", "Торос", "Тофик", "Трифон", "Трофим", "Тудхалия", "Тутмос", "Тьерри", "Тьяго", "Уве", "Уильям", "Улдис", "Ульрих", "Ульф", "Умар", "Урызмаг", "Усама", "Усман", "Фавст", "Фаддей", "Файзулла", "Фарид", "Фахраддин", "Федериго", "Федосей", "Федот", "Фейсал", "Феликс", "Феоктист", "Феофан", "Феофил", "Феофилакт", "Фердинанд", "Ференц", "Фёдор", "Фидель", "Филарет", "Филат", "Филип", "Филипп", "Философ", "Филострат", "Фирс", "Фока", "Фома", "Фотий", "Франц", "Франческо", "Фредерик", "Фридрих", "Фродо", "Фрол", "Фульк", "Хайме", "Ханс", "Харальд", "Харитон", "Харри", "Харрисон", "Хасан", "Хетаг", "Хильдерик", "Хирам", "Хлодвиг", "Хокон", "Хорив", "Хоселито", "Хосрой", "Хрисанф", "Христофор", "Хуан", "Цэрэндорж", "Чеслав", "Шалом", "Шамиль", "Шамсуддин", "Шапур", "Шарль", "Шейх-Хайдар", "Шон", "Эберхард", "Эдмунд", "Эдна", "Эдуард", "Элбэгдорж", "Элджернон", "Элиас", "Эллиот", "Эмиль", "Энрик", "Энрико", "Энтони", "Эразм", "Эраст", "Эрик", "Эрнст", "Эсекьель", "Эстебан", "Этьен", "Ювеналий", "Юлиан", "Юлий", "Юлиус", "Юрий", "Юстас", "Юстин", "Яков", "Якуб", "Якун", "Ян", "Яни", "Януарий", "Яромир", "Ярополк", "Ярослав", "Августа", "Августина", "Авдотья", "Аврелия", "Аврея", "Аврора", "Агапа", "Агапия", "Агарь", "Агата", "Агафа", "Агафия", "Агафоклия", "Агафоника", "Агафья", "Агита", "Аглаида", "Аглая", "Агна", "Агнесса", "Агния", "Аграфена", "Агриппина", "Ада", "Аделаида", "Аделина", "Аделия", "Аделла", "Адель", "Адельфина", "Адиля", "Адина", "Адолия", "Адриана", "Аза", "Азалия", "Азелла", "Азиза", "Аида", "Айжан", "Айта", "Акгюль", "Акилина", "Аксиния", "Аксинья", "Акулина", "Алана", "Алевтина", "Александра", "Александрина", "Алексина", "Алена", "Алеся", "Алешан", "Алёна", "Алико", "Алина", "Алиса", "Алла", "Алсу", "Алфея", "Альберта", "Альбертина", "Альбина", "Альвина", "Альжбета", "Альфия", "Альфреа", "Альфреда", "Амалия", "Амата", "Амелия", "Амелфа", "Амина", "Анабела", "Анастасия", "Анатолия", "Ангела", "Ангелика", "Ангелина", "Анджела", "Андрея", "Андрона", "Андроника", "Анжела", "Анжелика", "Анисия", "Анисья", "Анита", "Анна", "Антигона", "Антониана", "Антонида", "Антонина", "Антония", "Ануш", "Анфима", "Анфиса", "Анфия", "Анфуса", "Анэля", "Аполлинария", "Аполлония", "Апраксин", "Апрелия", "Апфия", "Арабелла", "Аргентея", "Ариадна", "Арина", "Ария", "Арлета", "Арминия", "Арсения", "Артемида", "Артемия", "Архелия", "Асия", "Аста", "Астра", "Ася", "Аурелия", "Афанасия", "Аэлита", "Бабетта", "Багдагуль", "Барбара", "Беата", "Беатриса", "Белла", "Бенедикта", "Береслава", "Бернадетта", "Берта", "Бибиана", "Биргит", "Бирута", "Бландина", "Бланка", "Богдана", "Божена", "Болеслава", "Борислава", "Ботогоз", "Бояна", "Бригитта", "Бронислава", "Бруна", "Валенсия", "Валентина", "Валерия", "Валида", "Валия", "Ванда", "Варвара", "Варя", "Васёна", "Васила", "Василида", "Василина", "Василиса", "Василия", "Василла", "Васса", "Вацлава", "Вевея", "Веджиха", "Велимира", "Велислава", "Венедикта", "Венера", "Венуста", "Венцеслава", "Вера", "Вербния", "Вереника", "Вероника", "Веселина", "Веста", "Вестита", "Вета", "Вива", "Вивея", "Вивиана", "Вида", "Видина", "Викентия", "Виктбрия", "Викторина", "Виктория", "Вила", "Вилена", "Виленина", "Вилора", "Вильгельмина", "Виолетта", "Виргиния", "Виринея", "Вита", "Виталика", "Виталина", "Виталия", "Витольда", "Влада", "Владилена", "Владимира", "Владислава", "Владлена", "Воислава", "Воля", "Всеслава", "Габриэлла", "Гаджимет", "Газама", "Гала", "Галата", "Галатея", "Гали", "Галима", "Галина", "Галла", "Галя", "Гая", "Гаянэ", "Геласия", "Гелена", "Гелла", "Гемелла", "Гемина", "Гения", "Геннадия", "Геновефа", "Генриетта", "Георгина", "Гера", "Германа", "Гертруда", "Гея", "Гизелла", "Глафира", "Гликерия", "Глорибза", "Глория", "Голиндуха", "Гольпира", "Гонеста", "Гонората", "Горгония", "Горислава", "Гортензия", "Градислава", "Гражина", "Грета", "Гулара", "Гульмира", "Гульназ", "Гульнара", "Гюзель", "Дайна", "Далила", "Далия", "Дамира", "Дана", "Даная", "Даниэла", "Данута", "Дариа", "Дарина", "Дария", "Дарья", "Дастагуль", "Дебора", "Деена", "Декабрена", "Денесия", "Денница", "Дея", "Джамиля", "Джана", "Джафара", "Джемма", "Джулия", "Джульетта", "Диана", "Дигна", "Диля", "Диляра", "Дина", "Динара", "Диодора", "Дионина", "Дионисия", "Дия", "Доброгнева", "Добромила", "Добромира", "Доброслава", "Доля", "Доминика", "Домитилла", "Домна", "Домника", "Домникия", "Домнина", "Донара", "Доната", "Дора", "Доротея", "Дорофея", "Доса", "Досифея", "Дросида", "Дуклида", "Ева", "Евангелина", "Еванфия", "Евгения", "Евдокия", "Евдоксия", "Евлалия", "Евлампия", "Евмения", "Евминия", "Евника", "Евникия", "Евномия", "Евпраксия", "Евсевия", "Евстафия", "Евстолия", "Евтихия", "Евтропия", "Евфалия", "Евфимия", "Евфросиния", "Екатерина", "Елена", "Елизавета", "Еликонида", "Епистима", "Епистимия", "Ермиония", "Есения", "Ефимия", "Ефимья", "Ефросиния", "Ефросинья", "Жанна", "Жеральдина", "Жозефина", "Забава", "Заира", "Замира", "Зара", "Зарема", "Зари", "Зарина", "Зарифа", "Звезда", "Земфира", "Зенона", "Зина", "Зинаида", "Зинат", "Зиновия", "Зита", "Злата", "Зоя", "Зульфия", "Зураб", "Зухра", "Ива", "Иванна", "Иветта", "Ивона", "Ида", "Идея", "Изабелла", "Изида", "Изольда", "Илария", "Илзе", "Илия", "Илона", "Ильина", "Ильмира", "Инара", "Инга", "Инесса", "Инна", "Иоанна", "Иовилла", "Иола", "Иоланта", "Ипполита", "Ирада", "Ираида", "Ирена", "Ирина", "Ирма", "Исидора", "Ифигения", "Июлия", "Ия", "Каздоя", "Казимира", "Калерия", "Калида", "Калиса", "Каллиникия", "Каллиста", "Каллисфения", "Кама", "Камила", "Камилла", "Кандида", "Капитолина", "Карима", "Карина", "Каролина", "Касиния", "Катарина", "Келестина", "Керкира", "Кетевань", "Кикилия", "Кима", "Кира", "Кириакия", "Кириана", "Кирилла", "Кирьяна", "Клавдия", "Клара", "Клариса", "Клементина", "Клена", "Клеопатра", "Климентина", "Клотильда", "Конкордия", "Констанция", "Консуэлла", "Кора", "Корнелия", "Кристина", "Ксаверта", "Ксанфиппа", "Ксения", "Купава", "Лавиния", "Лавра", "Лада", "Лайма", "Лариса", "Латафат", "Лаура", "Лебния", "Леда", "Лейла", "Лемира", "Ленина", "Леокадия", "Леонида", "Леонила", "Леонина", "Леонтина", "Леся", "Летиция", "Лея", "Лиана", "Ливия", "Лидия", "Лилиана", "Лилия", "Лина", "Линда", "Лира", "Лия", "Лола", "Лолита", "Лонгина", "Лора", "Лота", "Луиза", "Лукерья", "Лукиана", "Лукия", "Лукреция", "Любава", "Любовь", "Любомила", "Любомира", "Людмила", "Люсьена", "Люцина", "Люция", "Мавра", "Магда", "Магдалена", "Магдалина", "Магна", "Мадина", "Мадлена", "Маина", "Майда", "Майя", "Макрина", "Максима", "Малания", "Малика", "Малина", "Малинья", "Мальвина", "Мамелфа", "Манана", "Манефа", "Мануэла", "Маргарита", "Мариам", "Мариамна", "Мариана", "Марианна", "Мариетта", "Марина", "Маринэ", "Марионелла", "Марионилла", "Марица", "Мариэтта", "Мария", "Марка", "Маркеллина", "Маркиана", "Марксина", "Марлена", "Марселина", "Марта", "Мартина", "Мартиниана", "Марфа", "Марьина", "Марья", "Марьям", "Марьяна", "Мастридия", "Матильда", "Матрёна", "Матрона", "Мая", "Медея", "Мелания", "Меланья", "Мелитика", "Меркурия", "Мерона", "Милана", "Милена", "Милица", "Милия", "Милослава", "Милютина", "Мина", "Минна", "Минодора", "Мира", "Мирдза", "Миропия", "Мирослава", "Мирра", "Митродора", "Михайлина", "Михалина", "Млада", "Модеста", "Моика", "Моника", "Мстислава", "Муза", "Мэрилант", "Нада", "Надежда", "Назира", "Наиля", "Наина", "Нана", "Наркисса", "Настасия", "Настасья", "Наталия", "Наталья", "Нателла", "Нелли", "Ненила", "Неонила", "Нида", "Ника", "Нила", "Нимфа", "Нимфодора", "Нина", "Нинель", "Новелла", "Нонна", "Нора", "Норгул", "Ноэми", "Ноябрина", "Нунехия", "Одетта", "Оксана", "Октавия", "Октябрина", "Олдама", "Олеся", "Оливия", "Олимпиада", "Олимпиодора", "Олимпия", "Ольвия", "Ольга", "Ольда", "Офелия", "Павла", "Павлина", "Паисия", "Паллада", "Паллидия", "Пальмира", "Памела", "Параскева", "Патрикия", "Патриция", "Паула", "Паулина", "Пелагея", "Перегрина", "Перпетуя", "Петра", "Петрина", "Петронилла", "Петрония", "Пиама", "Пинна", "Плакида", "Плакилла", "Платонида", "Победа", "Полактия", "Поликсена", "Поликсения", "Полина", "Поплия", "Правдина", "Прасковья", "Препедигна", "Прискилла", "Просдока", "Пульхерия", "Пульхерья", "Рада", "Радана", "Радислава", "Радмила", "Радомира", "Радосвета", "Радослава", "Радость", "Раиса", "Рафаила", "Рахиль", "Рашам", "Ревекка", "Ревмира", "Регина", "Резета", "Рема", "Рената", "Римма", "Рипсимия", "Роберта", "Рогнеда", "Роза", "Розалина", "Розалинда", "Розалия", "Розамунда", "Розина", "Розмари", "Роксана", "Романа", "Ростислава", "Ружена", "Рузана", "Румия", "Русана", "Русина", "Руслана", "Руфина", "Руфиниана", "Руфь", "Сабина", "Савватия", "Савелла", "Савина", "Саида", "Саломея", "Салтанат", "Самона", "Сания", "Санта", "Сарра", "Сатира", "Светислава", "Светлана", "Светозара", "Святослава", "Севастьяна", "Северина", "Секлетея", "Секлетинья", "Селена", "Селестина", "Селина", "Серафима", "Сибилла", "Сильва", "Сильвана", "Сильвестра", "Сильвия", "Сима", "Симона", "Синклитикия", "Сиотвия", "Сира", "Слава", "Снандулия", "Снежана", "Созия", "Сола", "Соломонида", "Сосипатра", "София", "Софрония", "Софья", "Сталина", "Станислава", "Стелла", "Степанида", "Стефанида", "Стефания", "Сусанна", "Суфия", "Сюзанна", "Тавифа", "Таира", "Таисия", "Таисья", "Тала", "Тамара", "Тарасия", "Татьяна", "Тахмина", "Текуса", "Теодора", "Тереза", "Тигрия", "Тина", "Тихомира", "Тихослава", "Тома", "Томила", "Транквиллина", "Трифена", "Трофима", "Улдуза", "Улита", "Ульяна", "Урбана", "Урсула", "Устина", "Устиния", "Устинья", "Фабиана", "Фавста", "Фавстина", "Фаиза", "Фаина", "Фанни", "Фантика", "Фаня", "Фарида", "Фатима", "Фая", "Фебния", "Феврония", "Февронья", "Федоза", "Федора", "Федосия", "Федосья", "Федотия", "Федотья", "Федула", "Фекла", "Фекуса", "Феликса", "Фелица", "Фелицата", "Фелициана", "Фелицитата", "Фелиция", "Феогния", "Феодора", "Феодосия", "Феодота", "Феодотия", "Феодула", "Феодулия", "Феозва", "Феоктиста", "Феона", "Феонилла", "Феопистия", "Феосовия", "Феофания", "Феофила", "Фервуфа", "Феруза", "Фессалоника", "Фессалоникия", "Фетиния", "Фетинья", "Фея", "Фёкла", "Фива", "Фивея", "Филарета", "Филиппа", "Филиппин", "Филиппина", "Филомена", "Филонилла", "Филофея", "Фиста", "Флавия", "Флёна", "Флора", "Флорентина", "Флоренция", "Флориана", "Флорида", "Фомаида", "Фортуната", "Фотина", "Фотиния", "Фотинья", "Франсуаза", "Франциска", "Франческа", "Фредерика", "Фрида", "Фридерика", "Хаврония", "Халима", "Хариесса", "Хариса", "Харита", "Харитина", "Хильда", "Хильдегарда", "Хиония", "Хриса", "Хрисия", "Христиана", "Христина", "Христя", "Цвета", "Цветана", "Целестина", "Цецилия", "Чеслава", "Чулпан", "Шангуль", "Шарлотта", "Ширин", "Шушаника", "Эвелина", "Эгина", "Эдда", "Эдит", "Эдита", "Элахе", "Элеонора", "Элиана", "Элиза", "Элизабет", "Элина", "Элисса", "Элла", "Эллада", "Эллина", "Элоиза", "Эльвира", "Эльга", "Эльза", "Эльмира", "Эмилиана", "Эмилия", "Эмма", "Эннафа", "Эра", "Эрика", "Эрнеста", "Эрнестина", "Эсмеральда", "Эстер", "Эсфирь", "Юдита", "Юдифь", "Юзефа", "Юлдуз", "Юлиана", "Юлиания", "Юлия", "Юна", "Юния", "Юнона", "Юрия", "Юстина", "Юханна", "Ядвига", "Яна", "Янина", "Янита", "Янка", "Янсылу", "Ярослава" };

        public static string GetName()
        {
            string outstr = string.Empty;

            outstr += names[new Random((int)DateTime.Now.TimeOfDay.Ticks).Next(0, names.Length)];

            return outstr;
        }
    }

    abstract class MyRand
    {
        protected Random random;
        public int Core
        {
            get
            {
                return Core;
            }
            set
            {
                random = new Random(value);
                Core = value;
            }
        }
        public MyRand(int core = 67)
        {
            Core = core;
        }
        public abstract double Next();
    }
    class MyNorm : MyRand
    {
        public MyNorm(int core = 67)
        {
            base.Core = core;
        }
        public double Next(int expected, int dispersion, int count = 12)
        {
            double sum = 0;
            double result = 0;
            for (int i = 0; i < count; i++)
            {
                sum += (random.NextDouble() - 0.5);
            }

            result = Math.Sqrt((double)12 / (double)sum) * sum;

            if (expected != 0 || dispersion != 0)
            {
                result = dispersion * result + expected;
            }

            return result;
        }

        public override double Next()
        {
            return Next(0, 0);
        }
    }
    class MyUniform : MyRand
    {
        public MyUniform(int core = 67)
        {
            base.Core = core;
        }
        public double Next(int a, int b)
        {
            return random.Next(a, b);
        }

        public override double Next()
        {
            return Next(0, 0);
        }
    }
}