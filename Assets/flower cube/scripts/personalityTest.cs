using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class personalityTest : MonoBehaviour
{
    // each question is stored as a State, with all info about that question (next states, answers, etc)
    public struct State
    {
        public State(int state_no, int to_0_state, int to_1_state, string question)
        {
            q = question;

            current_st = state_no;
            next_st_0 = to_0_state;
            next_st_1 = to_1_state;
        }

        public string q { get; }

        public int current_st { get; }
        public int next_st_0 { get; }
        public int next_st_1 { get; }

        // show question & answer text
        public void printText()
        {
            q_text.text = q;
        }

        // return next state based on which answer is chosen
        public int nextState(int btn_pressed)
        {
            return ((btn_pressed == 0) ? next_st_0 : next_st_1);
        }
    };

    // the 11 different end states are also stored as structs
    public struct endState
    {
        public endState(GameObject end_cube, string end_title, string end_text)
        {
            cube = end_cube;
            title = end_title;
            body_text = end_text;
        }

        public GameObject cube { get; }
        public string title { get; }
        public string body_text { get; }

        // activate final scene & corresponding text
        public void endScreen()
        {
            cube.SetActive(true);
            end_title_text.text = title;
            end_body_text.text = body_text;
        }

        // hides cube from this particular end state
        public void deactivateCube()
        {
            cube.SetActive(false);
        }
    }
    
    // All UI text & objects
    static TMP_Text q_text, a0_text, a1_text, end_title_text, end_body_text;
    public GameObject start_ui, question_ui, answer_0_ui, answer_1_ui, end_ui, end_frame;
    public GameObject end_cube_1, end_cube_2, end_cube_3, end_cube_4, end_cube_5, end_cube_6;
    public Sprite start_glow, yes_glow, no_glow, back_glow, restart_glow;

    // Stuff to control the various state machines
    private int state, num_questions;
    private bool started, ended;

    // stack to record previously visited states (for undo button)
    private Stack<int> prev_states = new Stack<int>();

    // arrays to hold states & sprites
    private State[] state_array;
    private endState[] end_state_array;
    private Sprite[] glow_sprites;
    

    void Start()
    {
        // get text from various UI panels (has to happen in start bc of the structs)
        q_text = question_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        end_title_text = end_ui.transform.GetChild(1).GetComponent<TMP_Text>();
        end_body_text = end_ui.transform.GetChild(2).GetComponent<TMP_Text>();

        // initialize state array with all states & text
        state_array = new State[] { new State(1, 2, 4,
                                        "여러가지 생각이 많은 편이다"), 
                                    new State(2, 3, 5,
                                        "소통을 잘 한다"),
                                    new State(3, 19, 6,
                                        "새로운 지식을 배우는 것이 즐겁다"),
                                    new State(4, 5, 7,
                                        "자기관리가 어렵다"),
                                    new State(5, 6, 8,
                                        "에너지가 넘친다"),
                                    new State(6, 20, 9,
                                        "인간관계가 넓다"),
                                    new State(7, 8, 10,
                                        "감정 컨트롤을 잘하는 편이다"),
                                    new State(8, 9, 11,
                                        "공감능력이 좋다"),
                                    new State(9, 21, 12,
                                        "조용한 성격이다"),
                                    new State(10, 11, 13,
                                        "협업 보다 혼자하는 게 편하다"),
                                    new State(11, 12, 14,
                                        "반복적인 것이 싫다"),
                                    new State(12, 22, 15,
                                        "아이디어가 많다"),
                                    new State(13, 14, 16,
                                        "의견을 말하는데 어려움이 없다"),
                                    new State(14, 15, 17,
                                        "리더쉽이 있다"),
                                    new State(15, 23, 18,
                                        "맡은 일에 책임감이 강하다"),
                                    new State(16, 17, 14,
                                        "나는 나보다 남을 더 신경쓴다"),
                                    new State(17, 18, 15,
                                        "난 운명적인 사랑을 믿는다"),
                                    new State(18, 24, 23,
                                        "감수성이 풍부하다"),
                                    };

        // initialize end state array
        end_state_array = new endState[] {  new endState(end_cube_1,
                                                "균형과 신중함을 가진 붓꽃속",
                                                "당신은 조화롭고 안정된 성격을 갖추고 있으며 사려깊고 균형을 잘 잡는 능력을 가지고 있습니다. 분석적이고 논리적인 사고를 통해 문제를 해결하고자 하는 경향이 있으며, 세부적인 사항을 주의깊게 살펴보고 최선의 결정을 내리려고 합니다. 이러한 특성으로 인해 당신은 신뢰할 만한 사람으로 인정받을 수 있습니다. 지적 호기심이 강하고 지식을 쌓는 것을 즐기는 경향도 있으며, 학습과 성장에 대한 열망이 있습니다. 하지만 때로는 너무 신중하거나 과도한 완벽주의에 빠지는 경향이 있을 수 있습니다. 결정을 내리는 데 시간이 오래 걸리거나, 조급함을 피하고 자신의 판단에 자신을 신뢰하는 것이 중요합니다. "),
                                            new endState(end_cube_2,
                                                "언제나 밝고 심지 굳은 해바라기",
                                                "당신은 낙관적이고 활기찬 사람으로, 주변 사람들에게 긍정적인 영향력을 행사하며 에너지와 열정으로 일을 추진하는 능력을 가지고 있습니다. 목표를 설정하고 꾸준히 노력하여 성취를 이루는 경향이 있으며 어려운 상황에서도 긍정적으로 대처할 수 있습니다. 하지만 때로는 자신을 희생하는 경향이 있을 수 있으니 자신의 경계를 지키고 자기관리와 자기돌봄에 충분한 시간과 에너지를 할애하는 것이 중요합니다. "),  
                                            new endState(end_cube_3,
                                                "차분하고 우아한 흰장미",
                                                "당신은 순수성과 깨끗함을 상징하는 흰장미와 같이 자신의 내면을 보호하고 균형을 유지하려는 노력을 합니다. 섬세하고 예리한 관찰력을 가지며, 친절하고 배려심이 넘치고 주변 사람들과의 관계에 중요한 가치를 둡니다. 예술적인 감각과 미적 감각을 사랑하는 경향이 있으며, 창조적인 분야에서 뛰어난 성과를 이뤄낼 수 있습니다. 어려운 상황에서도 담담하게 대처할 수 있는 능력을 갖고 있으며, 주변 사람들은 당신의 조언과 지지를 믿고 의지할 수 있습니다. 다만 가끔은 과도한 완벽주의에 빠지는 경향이 있으니 자신의 한계를 인정하고 균형을 유지하는 것이 중요합니다. "),  
                                            new endState(end_cube_4,
                                                "개성적이고 독특한 양귀비",
                                                "당신은 보통과 다른 것을 좋아하며, 창의적이고 독립적인 결정을 내릴 수 있는 자신감이 있습니다. 다른 사람들의 의견에 크게 영향받지 않고, 일상적인 틀을 벗어나 자신만의 독특한 방식으로 생각하고 행동합니다. 또한 호기심이 많고 성장을 추구하는 태도를 가지고 있으며, 새로운 지식과 기술을 습득하는 데 열정적입니다. 당신은 자유로운 영혼이며, 자신의 가치와 믿음을 소중히 여깁니다.  하지만 때로는 다른 사람들과 조화롭게 일하는 데 어려움을 느낄 수 있으니 타인의 의견을 수용하고 협력하는 능력을 발전시키는 것이 중요합니다. 자신을 이해하고 독특한 개성을 존중하며 성장해 나가길 바랍니다."),  
                                            new endState(end_cube_5,
                                                "강인하고 용감한 글라디올러스",
                                                "어떤 어려움이든 꾸준히 노력하며 극복하려는 끈기와 인내심을 가지고 있습니다. 능동적이고 자기 주도적인 성격으로 주변 사람들의 존경과 신뢰를 받고 있습니다. 당신은 자신에게 높은 목표와 꿈을 세우고, 그것을 달성하기 위해 노력하는 우매하고 결단력 있는 사람입니다. 이러한 특성은 당신이 성취 지향적이고, 문제 해결에 능숙한 사람임을 나타냅니다.사람들은 당신을 존경하며, 당신의 영향력으로 인해 변화와 성장을 이루는 데 도움이 됩니다. 하지만 때로는 집요하거나 완벽주의에 빠지는 경향이 있으므로 자기 자신을 너그럽게 받아들이고 완벽을 허용하는 능력을 발전시키는 것이 중요합니다. 강인함과 결단력을 유지하며 성장하길 바랍니다."),  
                                            new endState(end_cube_6,
                                                "섬세하고 로맨틱한 아몬드꽃",
                                                "예술적인 영감과 따뜻한 감성을 지니며, 주변 사람들은 당신의 섬세한 성격과 배려심을 인정합니다. 당신의 존재는 분위기를 밝히고 따뜻함을 전달합니다. 또한 민감하고 감수성이 풍부해 다른 사람들의 감정을 잘 이해하고 공감합니다. 이러한 특성으로 인해 당신은 주변 사람들과의 관계에서 따뜻한 연결을 형성하며, 친구들과 가족들에게 신뢰와 지지를 주는 중요한 역할을 수행합니다. 당신은 변화를 받아들이고 새로운 경험을 즐기며, 어려운 상황에서도 긍정적으로 대처할 수 있는 힘을 가지고 있습니다. 미래에 대한 희망과 목표를 가지고 있으며, 꿈을 향해 진취적으로 나아가는 모습이 인상적입니다. 단, 감정에 휩싸이지 않도록 주의하고 감정과 이성을 균형있게 조화시키는 것이 중요합니다.")
                                        };   

        // initialize glow button sprite array
        glow_sprites = new Sprite[] {start_glow, yes_glow, no_glow, back_glow, restart_glow}; 

        // test prep stuff
        num_questions = state_array.Length;
        startTest();
    }

    void Update()
    {
        if (ended)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                // restart test if ended & restart button pressed
                StartCoroutine(restartTest());
            }
            else return;
        }
        else if (started)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A))
            {
                // first answer chosen, go to next question
                StartCoroutine(changeStates(0));
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                // second answer chosen, go to next question
                StartCoroutine(changeStates(1));
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.X))
            {
                // back button chosen
                StartCoroutine(changeStates(2));
            }
        }
        else if (!started && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            // begin test (from start screen)
            StartCoroutine(enterTest());
        }
    }

    IEnumerator pressButton(Transform button, int index) 
    {
        // simple button press color animation (turns green for 0.5 seconds)
        Sprite old_sprite = button.GetComponent<SpriteRenderer>().sprite;

        button.GetComponent<SpriteRenderer>().sprite = glow_sprites[index];

        yield return new WaitForSeconds(0.5f);
            
        button.GetComponent<SpriteRenderer>().sprite = old_sprite;
    }


    void startTest()
    {
        // configure UI panel for starting screen
        start_ui.SetActive(true);
        answer_0_ui.SetActive(false);
        answer_1_ui.SetActive(false);
        question_ui.SetActive(false);
        end_ui.SetActive(false);
        end_frame.SetActive(false);
        
        started = false;
        ended = false;

        state = 1;
    }

    IEnumerator enterTest()
    {
        yield return pressButton(start_ui.transform, 0);
        
        // configure UI for beginning test (going to first question)
        start_ui.SetActive(false);
        answer_0_ui.SetActive(true);
        answer_1_ui.SetActive(true);
        question_ui.SetActive(true);
        end_ui.SetActive(false);
        end_frame.SetActive(false);

        started = true;
        ended = false;

        state_array[state - 1].printText();
    }

    IEnumerator changeStates(int btn_press)
    {
        if (btn_press == 2)
        {
            // if redo, go back to previous state
            yield return pressButton(question_ui.transform.GetChild(1), 3);

            state = prev_states.Pop();
            state_array[state - 1].printText();
        }
        else 
        {
            // get pressed button
            Transform btn_bkg = (btn_press == 0) ? answer_0_ui.transform : answer_1_ui.transform;
            yield return pressButton(btn_bkg, (btn_press == 0) ? 1 : 2);

            // add state to stack & move to next one
            prev_states.Push(state);
            state = state_array[state - 1].nextState(btn_press);

            if (state > num_questions)
            {
                // if at the end of the test, go to ending screen
                endTest();
            }
            else
            {
                state_array[state - 1].printText();
            }
        }
    }

    void endTest()
    {
        // configure UI for end of test
        start_ui.SetActive(false);
        answer_0_ui.SetActive(false);
        answer_1_ui.SetActive(false);
        question_ui.SetActive(false);
        end_frame.SetActive(true);
        end_ui.SetActive(true);

        started = true;
        ended = true;

        end_state_array[state - 1 - num_questions].endScreen();
    }

    IEnumerator restartTest()
    {
        yield return pressButton(end_ui.transform.GetChild(3), 4);

        // erase answer cube & restart test
        end_state_array[state - 1 - num_questions].deactivateCube();
        
        startTest();
    }
}
