﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inheritance
{
    // 추상멤버를 가지고있다
    // -> 해당 멤버를 가지는 클래스가 인스턴스화 가능하다면 해당 함수를 호출할 수 있어야하는데 모순이 생긴다.
    // -> 추상멤버를 가지고 있으면, 클래스 마찬가지로 추상 클래스여야한다.
    internal abstract class Character : Creature, IDamageable
    {
        // 프로퍼티 
        public float hp
        {
            get
            {
                return _hp;
            }
            set
            {
                if (value < 0)
                    value = 0;
              
                if (_hp == value)
                    return;
                
                _hp = value;

                //if (onHpChanged != null)
                //    onHpChanged(value); // 직접호출 , 결과이상이 생길수도 있음
                //
                //onHpChanged.Invoke(value); // 간접호출 , 결과보장이됨
                onHpChanged?.Invoke(value); // ? : Null check 연산자
            }
        }

        private float _hp;
        public delegate void HpChangedHandler(float value); // delegate : 대리자 <특별하게 제공해야하는 정보를 쓸떄 쓰는게 좋음>
        public event HpChangedHandler onHpChanged;          // 함수 체이닝을 참조한다.
        public delegate void SomeHandler<in T1, in T2>(T1 value1, T2 value2);
        public event SomeHandler<double, int> onSomeChanged;
        // event : 한정자
        // 대리자의 접근 제한을 위한 한정자
        // +=, -= (구독 / 구독취소) 는 외부 클래스에서 호출 가능.
        // 구동에 대한 표현 : Register, Observe, Listen, Subscribe 다 똑같은 말
        // event 를 호출한다는 표현 : Notify (구독자들에게 알림통보)
        // = 는 접근 불가능
        public event Action action1;
        public event Action<float> action2;
        public event Action<int, string> action3;

        public event Func<int> func1;
        public event Func<float, int> func2;
        public event Func<float, double, int> func3;
        public event Func<int, bool> func4;

        public event Predicate<int> predicate; // = func<int, bool>
        

        //public float GetHp()
        //{
        //    return _hp;
        //}
        //
        //public void SetHp(float value) 
        //{
        //    if (value < 0)
        //        value = 0;
        //
        //    _hp = value;
        //}
        //public void Damage(float amount)
        //{
        //    hp -= amount;
        //}
        public void Damage(float amount)
        {
            hp -= amount; // == hp = hp - amount;
        }
        public abstract void UniqueSkill(); // 추상함수는 정의하지않고 선언만 함.
    }
}
