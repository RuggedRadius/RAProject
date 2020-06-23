
using RAProject.Models;

namespace RAProject.Utilities
{
    public class MergeSort
    {
        static public void DoMerge_Games(Game[] numbers, int left, int mid, int right)
        {
            Game[] temp = new Game[numbers.Length];
            int left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (numbers[left].Title.CompareTo(numbers[mid].Title) < 0)
                    temp[tmp_pos++] = numbers[left++];
                else
                    temp[tmp_pos++] = numbers[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = numbers[left++];

            while (mid <= right)
                temp[tmp_pos++] = numbers[mid++];

            for (int i = 0; i < num_elements; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }
        public static void Games_Rescursive(Game[] numbers, int left, int right)
        {
            int mid;

            if (right.CompareTo(left) > 0)
            {
                mid = (right + left) / 2;
                Games_Rescursive(numbers, left, mid);
                Games_Rescursive(numbers, (mid + 1), right);

                DoMerge_Games(numbers, left, (mid + 1), right);
            }
        }

        static public void DoMerge_Consoles(GameConsole[] numbers, int left, int mid, int right)
        {
            GameConsole[] temp = new GameConsole[numbers.Length];
            int left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (numbers[left].Name.CompareTo(numbers[mid].Name) < 0)
                    temp[tmp_pos++] = numbers[left++];
                else
                    temp[tmp_pos++] = numbers[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = numbers[left++];

            while (mid <= right)
                temp[tmp_pos++] = numbers[mid++];

            for (int i = 0; i < num_elements; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }
        public static void Consoles_Rescursive(GameConsole[] numbers, int left, int right)
        {
            int mid;

            if (right.CompareTo(left) > 0)
            {
                mid = (right + left) / 2;
                Consoles_Rescursive(numbers, left, mid);
                Consoles_Rescursive(numbers, (mid + 1), right);

                DoMerge_Consoles(numbers, left, (mid + 1), right);
            }
        }

        static public void DoMerge_Achievements(Achievement[] numbers, int left, int mid, int right)
        {
            Achievement[] temp = new Achievement[numbers.Length];
            int left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (numbers[left].ID.CompareTo(numbers[mid].ID) < 0)
                    temp[tmp_pos++] = numbers[left++];
                else
                    temp[tmp_pos++] = numbers[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = numbers[left++];

            while (mid <= right)
                temp[tmp_pos++] = numbers[mid++];

            for (int i = 0; i < num_elements; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }
        public static void Achievements_Rescursive(Achievement[] numbers, int left, int right)
        {
            int mid;

            if (right.CompareTo(left) > 0)
            {
                mid = (right + left) / 2;
                Achievements_Rescursive(numbers, left, mid);
                Achievements_Rescursive(numbers, (mid + 1), right);

                DoMerge_Achievements(numbers, left, (mid + 1), right);
            }
        }




    }
}