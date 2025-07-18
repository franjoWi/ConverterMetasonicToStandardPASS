using alps.net.api.ALPS;
using alps.net.api.StandardPASS;
using Converter.parsing.MetasonicProcessModelElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.converting
{
    internal class PASSConverterMetasonicToStandard
    {
        public MetasonicProcessModel MetasonicModel { get; }
        public PASSProcessModel OWLStandardModel { get; set; }
        public ModelLayer StandardLayer { get; set; }

        /// <summary>
        /// Loads the elements and attributes of the actual Metasonic process model into MetasonicModel.
        /// </summary>
        /// <param name="filepath">File path of the process model.</param>
        public PASSConverterMetasonicToStandard(string filepath) 
        {
            MetasonicModel = new MetasonicProcessModel(filepath);
        }

        /// <summary>
        /// Converts and exports the Metasonic porcess model.
        /// </summary>
        /// <param name="destination_filepath">Filepath where the output should be saved.</param>
        /// <param name="baseuri">Baseuri of the converted model.</param>
        public void convertMetasonicModelToOWLStandard(string destination_filepath, string baseuri)
        {
            initializeProcessModel(baseuri);
            addSubjects();
            addMessageSpecifications();
            addMessageExchangesAndLists();
            addStatesAndStartSubjects();
            addTransitions();
            
            OWLStandardModel.export(destination_filepath);
        }

        /// <summary>
        /// Process Description in Metasonic is being converted into a PASSProcessModel, which needs one layer.
        /// Metasonic Attributes to OWL Standard: id, name, comment
        /// </summary>
        /// <param name="baseuri"></param>
        private void initializeProcessModel(string baseuri)
        {
            OWLStandardModel = new PASSProcessModel(baseURI: baseuri, additionalLabel: MetasonicModel.Name, comment: MetasonicModel.Comment);
            OWLStandardModel.setModelComponentID(MetasonicModel.Id);
            StandardLayer = new ModelLayer(OWLStandardModel);
        }
        
        /// <summary>
        /// Subject Descriptions in Metasonic are being converted into a Subject in PASS.
        /// Metasonic Attributes to OWL Standard: id, name, position_type, role, roleName, is_subject_array, comment
        /// </summary>
        private void addSubjects()
        {
            foreach (MetasonicSubject s in MetasonicModel.Subjects)
            {
                SubjectBaseBehavior sb;
                Subject sub;

                if (s.PositionType == "internal")
                {
                    if (s.IsSubjectArray == "no")
                    {
                        sb = new SubjectBaseBehavior(StandardLayer);
                        sb.setModelComponentID(s.Id + "_sb");
                        sub = new FullySpecifiedSubject(StandardLayer, subjectBaseBehavior: sb, additionalLabel: s.Name, comment: s.Comment);
                        sub.setModelComponentID(s.Id);
                    }
                    else if (s.IsSubjectArray == "yes")
                    {
                        sb = new SubjectBaseBehavior(StandardLayer);
                        sb.setModelComponentID(s.Id + "_sb");
                        sub = new FullySpecifiedSubject(StandardLayer, subjectBaseBehavior: sb, maxSubjectInstanceRestriction: 2, additionalLabel: s.Name, comment: s.Comment); //maxSubjectInstanceRestriction is 2 as default because there is no attribute in Metasonic for that.
                        sub.setModelComponentID(s.Id);
                    }

                }
                else if (s.PositionType == "external")
                {
                    if (s.IsSubjectArray == "no")
                    {
                        sub = new InterfaceSubject(StandardLayer, additionalLabel: s.Name, comment: s.Comment);
                        sub.setModelComponentID(s.Id);
                    }
                    else if (s.IsSubjectArray == "yes")
                    {
                        sub = new InterfaceSubject(StandardLayer, maxSubjectInstanceRestriction: 2, additionalLabel: s.Name, comment: s.Comment);
                        sub.setModelComponentID(s.Id);
                    }
                }

                if (!(s.Role == "" || s.Role == null))
                {
                    SubjectExecutionMapping sem = new SubjectExecutionMapping(StandardLayer, additionalLabel: s.RoleName);
                    sem.setModelComponentID(s.Role);
                }
            }
        }

        /// <summary>
        /// MessageType Descriptions in Metasonic are being converted into a MessageType in PASS.
        /// Metasonic Attributes to OWL Standard: id, name, comment
        /// </summary>
        private void addMessageSpecifications()
        {
            foreach (MetasonicMessageType mt in MetasonicModel.MessageTypes)
            {
                MessageSpecification ms = new MessageSpecification(StandardLayer, additionalLabel: mt.Name, comment: mt.Comment);
                ms.setModelComponentID(mt.Id);
            }
        }

        /// <summary>
        /// Message Descriptions in Metasonic are being converted into a Message in PASS.
        /// Metasonic Attributes to OWL Standard: id, source, destination, name, comment
        /// </summary>
        private void addMessageExchangesAndLists()
        {
            foreach (MetasonicMessage msg in MetasonicModel.Messages)
            {
                int i = 0;
                foreach (MetasonicMessageType mt in msg.MessageTypes)
                {
                    //As Metasonic does not support the concept of a differentiation between MessageExchange and MessageExchangeList a new Id needs to be created.
                    MessageSpecification ms = (MessageSpecification)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == mt.Id).Value;
                    string newId = msg.Id + "_" + i++;

                    Subject sender = (Subject)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == msg.Source.Id).Value;
                    Subject receiver = (Subject)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == msg.Destination.Id).Value;

                    MessageExchange me = new MessageExchange(StandardLayer, messageSpecification: ms, senderSubject: sender, receiverSubject: receiver, comment: mt.Comment);
                    me.setModelComponentID(newId);
                }
            }
        }
        /// <summary>
        /// State Descriptions in Metasonic are being converted into States in Standard PASS. Also Start Subjects are calculated based on the States and added to OWLStandardModel.
        /// Metasonic Attributes to OWL Standard: id, name, comment, role, startstate, endstate, type
        /// </summary>
        private void addStatesAndStartSubjects()
        {
            foreach (MetasonicSubject su in MetasonicModel.Subjects)
            {
                SubjectBaseBehavior sb = (SubjectBaseBehavior)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == (su.Id + "_sb")).Value;
                ISet<IState> endStates = new HashSet<IState>();
                ISet<ISubject> startSubjects = new HashSet<ISubject>();
                foreach (MetasonicState st in su.States)
                {
                    State state;
                    switch (st.Type)
                    {
                        case "FUNCTION":
                            state = new DoState(sb, additionalLabel: st.Name, comment: st.Comment);
                            state.setModelComponentID(st.Id);
                            if (st.Endstate == "yes") endStates.Add(state);
                            if (st.Startstate == "yes")
                            {
                                sb.setInitialState(state);
                                startSubjects.Add(sb.getSubject());
                            }
                            break;
                        case "SEND":
                            state = new SendState(sb, additionalLabel: st.Name, comment: st.Comment);
                            state.setModelComponentID(st.Id);
                            if (st.Endstate == "yes") endStates.Add(state);
                            if (st.Startstate == "yes")
                            {
                                sb.setInitialState(state); ///???????????????????
                                startSubjects.Add(sb.getSubject());
                            }
                            break;
                        case "RECEIVE":
                            state = new ReceiveState(sb, additionalLabel: st.Name, comment: st.Comment);
                            state.setModelComponentID(st.Id);
                            if (st.Endstate == "yes") endStates.Add(state);
                            if (st.Startstate == "yes") sb.setInitialState(state);
                            break;
                    }
                    sb.setEndStates(endStates);
                    OWLStandardModel.setStartSubjects(startSubjects);
                }
            }
        }

        /// <summary>
        /// Transition Descriptions in Metasonic are being converted into Transitions in Standard PASS. Also Transition Conditions are being created.
        /// Metasonic Attributes to OWL Standard: id, name, comment, source, destination, positionId, event, timeout
        /// </summary>
        private void addTransitions()
        {
            foreach (MetasonicSubject su in MetasonicModel.Subjects)
            {
                foreach (MetasonicTransition tr in su.Transitions)
                {
                    State source = (State)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == (tr.Source.Id)).Value;
                    State destination = (State)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == (tr.Destination.Id)).Value;
                    Transition transition;
                    int.TryParse(tr.PositionId, out int positionId);
                    switch (tr.Event)
                    {
                        case "FUNCTION":
                            transition = new DoTransition(source, destination, priorityNumber: positionId, comment: tr.Comment);
                            transition.setModelComponentID(tr.Id);
                            addTransitionCondition(transition, tr);
                            break;
                        case "SEND":
                            transition = new SendTransition(source, destination, comment: tr.Comment);
                            transition.setModelComponentID(tr.Id);
                            addTransitionCondition(transition, tr);
                            break;
                        case "RECEIVE":
                            transition = new ReceiveTransition(source, destination, priorityNumber: positionId, comment: tr.Comment);
                            transition.setModelComponentID(tr.Id);
                            addTransitionCondition(transition, tr);
                            break;
                        case "TIMEOUT":
                            if (tr.Timeout == "0")
                            {
                                transition = new UserCancelTransition(source, destination, comment: tr.Comment);
                                transition.setModelComponentID(tr.Id);
                                addTransitionCondition(transition, tr);
                            }
                            else
                            {
                                transition = new TimeTransition(source, destination, timeTransitionType: ITimeTransition.TimeTransitionType.DayTimeTimer, comment: tr.Comment);
                                transition.setModelComponentID(tr.Id);
                                addTransitionCondition(transition, tr);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a transition condition for a specific Transition (in Standard PASS) based on the mapped transition in Metasonic.
        /// Metasonic Attributes to OWL-Standard: underlying_message_id, sendType, receiveType, min, max, comment, technicalName, event_type, event_subject_id
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="tr"></param>
        private void addTransitionCondition(Transition transition, MetasonicTransition tr)
        {
            string messageId = tr.UnderlyingMessageId;
            string messageTypeName = tr.EventType;
            List<KeyValuePair<string, IPASSProcessModelElement>> msgs = OWLStandardModel.getAllElements().Where(el => el.Key.StartsWith(messageId)).ToList();
            List<MessageExchange> messages = msgs.Select(el => el.Value).OfType<MessageExchange>().ToList();
            MessageExchange msg = messages.FirstOrDefault(el => el.getMessageType().getModelComponentLabelsAsStrings().Contains(messageTypeName));
            MessageSpecification msgSpec = (MessageSpecification)msg.getMessageType();
            Subject sub = (Subject)OWLStandardModel.getAllElements().FirstOrDefault(el => el.Key == tr.EventSubjectId).Value;
            switch (transition.getClassName())
            {
                case "DoTransition":
                    DoTransitionCondition transitionDoCondition = new DoTransitionCondition(transition);
                    break;
                case "SendTransition":
                    SendTransitionCondition transitionSendCondition = new SendTransitionCondition(transition, messageExchange: msg, messageSentFromSubject: sub, receptionOfMessage: msgSpec);
                    if (tr.SendType == "1")
                    {
                        string transitionId = new string(tr.Id.SkipWhile(char.IsLetter).ToArray());
                        MetasonicVariableDescription variable = tr.ParentSubject.VariableDescriptions.FirstOrDefault(el => el.TechnicalName.Contains(transitionId) && el.TechnicalName.Contains("minMaxtransition"));
                        transitionSendCondition.setMultipleSendLowerBound(variable.Min);
                        transitionSendCondition.setMultipleSendUpperBound(variable.Max);
                        transitionSendCondition.addComment(variable.Comment);
                        transitionSendCondition.setSendType(ISendTransitionCondition.SendTypes.SEND_TO_KNOWN);
                    }
                    else if (tr.SendType == "3")
                    {
                        string transitionId = new string(tr.Id.SkipWhile(char.IsLetter).ToArray());
                        MetasonicVariableDescription variable = tr.ParentSubject.VariableDescriptions.FirstOrDefault(el => el.TechnicalName.Contains(transitionId) && el.TechnicalName.Contains("minMaxtransition"));
                        transitionSendCondition.setMultipleSendLowerBound(variable.Min);
                        transitionSendCondition.setMultipleSendUpperBound(variable.Max);
                        transitionSendCondition.addComment(variable.Comment);
                        transitionSendCondition.setSendType(ISendTransitionCondition.SendTypes.SEND_TO_NEW);
                    }
                    else if (tr.SendType == "4")
                    {
                        transitionSendCondition.setSendType(ISendTransitionCondition.SendTypes.SEND_TO_ALL);
                    }
                    break;
                case "ReceiveTransition":
                    ReceiveTransitionCondition transitionReceiveCondition = new ReceiveTransitionCondition(transition, messageExchange: msg, requiredMessageSendFromSubject: sub, requiresReceptionOfMessage: msgSpec);
                    if (tr.ReceiveType == "1")
                    {
                        transitionReceiveCondition.setReceiveType(IReceiveTransitionCondition.ReceiveTypes.RECEIVE_FROM_ALL);
                    }
                    else if (tr.ReceiveType == "2")
                    {
                        string transitionId = new string(tr.Id.SkipWhile(char.IsLetter).ToArray());
                        MetasonicVariableDescription variable = tr.ParentSubject.VariableDescriptions.FirstOrDefault(el => el.TechnicalName.Contains(transitionId) && el.TechnicalName.Contains("minMaxtransition"));
                        transitionReceiveCondition.setMultipleReceiveLowerBound(variable.Min);
                        transitionReceiveCondition.setMultipleReceiveUpperBound(variable.Max);
                        transitionReceiveCondition.addComment(variable.Comment);
                        transitionReceiveCondition.setReceiveType(IReceiveTransitionCondition.ReceiveTypes.RECEIVE_FROM_KNOWN);
                    }
                    break;
                case "UserCancelTransition":
                    break;
                case "TimeTransition":
                    TimeTransitionCondition transitionTimeCondition = new TimeTransitionCondition(transition, toolSpecificDefintion: "Miliseconds: " + tr.Timeout, timeTransitionConditionType: ITimeTransitionCondition.TimeTransitionConditionType.TimerTC);
                    break;
            }
            

        }

        
    }
}
