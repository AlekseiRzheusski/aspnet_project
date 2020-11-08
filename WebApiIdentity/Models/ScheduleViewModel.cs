using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiIdentity.Models
{
    public class ScheduleViewModel
    {
        public List<TimeSpan> times = new List<TimeSpan>() { new TimeSpan(8, 30, 00), new TimeSpan(9, 30, 00), new TimeSpan(10, 30, 00), new TimeSpan(11, 35, 00), new TimeSpan(12, 35, 00), new TimeSpan(14, 30, 00) };



        public string classId { get; set; }
        public List<LessonViewModel> mondayLessons = new List<LessonViewModel>();
        public List<LessonViewModel> tuesdayLessons = new List<LessonViewModel>();
        public List<LessonViewModel> wednesdayLessons = new List<LessonViewModel>();
        public List<LessonViewModel> thursdayLessons = new List<LessonViewModel>();
        public List<LessonViewModel> fridayLessons = new List<LessonViewModel>();
        public int[] difficulty=new int[5];


        public void countDifficulty()
        {
            for(int i = 0; i < 5; i++)
            {
                difficulty[i] = 0;
            }
            foreach(var tmp in mondayLessons)
            {
                difficulty[0] += tmp.subject.dificuty;
            }
            foreach (var tmp in tuesdayLessons)
            {
                difficulty[1] += tmp.subject.dificuty;
            }
            foreach (var tmp in wednesdayLessons)
            {
                difficulty[2] += tmp.subject.dificuty;
            }
            foreach (var tmp in thursdayLessons)
            {
                difficulty[3] += tmp.subject.dificuty;
            }
            foreach (var tmp in fridayLessons)
            {
                difficulty[4] += tmp.subject.dificuty;
            }
        }
    }

}