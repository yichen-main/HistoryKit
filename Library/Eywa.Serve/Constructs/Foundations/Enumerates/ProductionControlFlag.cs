namespace Eywa.Serve.Constructs.Foundations.Enumerates;
public enum ProductionControlFlag
{
    ProduceGroupLabel,
    ProduceGroupIdIsRequired,
    ProduceGroupIdDoesNotExist,
    ProduceGroupNoIsRequired,
    ProduceGroupNoIndex,
    ProduceGroupNameIsRequired,

    ProduceMachineLabel,
    ProduceMachineIdIsRequired,
    ProduceMachineIdDoesNotExist,

    ProduceOperatorLabel,
    ProduceOperatorIdIsRequired,
    ProduceOperatorIdDoesNotExist,

    ProduceProcessLabel,
    ProduceProcessIdIsRequired,
    ProduceProcessIdDoesNotExist,
    ProduceProcessNoIsRequired,
    ProduceProcessNoIndex,
    ProduceProcessNameIsRequired,

    ProduceShiftLabel,
    ProduceShiftIdIsRequired,
    ProduceShiftIdDoesNotExist,
    ProduceShiftNoIsRequired,
    ProduceShiftNoIndex,
    ProduceShiftNameIsRequired,
    ProduceInvalidStartTimePointFormat,
    ProduceInvalidEndTimePointFormat,

    ProduceStationLabel,
    ProduceStationIdIsRequired,
    ProduceStationIdDoesNotExist,
    ProduceStationNoIsRequired,
    ProduceStationNoIndex,
    ProduceStationNameIsRequired,


    ManufactureOrderLabel,
    ManufactureOrderIdIsRequired,
    ManufactureOrderIdDoesNotExist,
    ManufactureOrderNoIsRequired,
    ManufactureOrderNoIndex,
    ManufactureOrderNameIsRequired,


    WorkingLocationLabel,
    WorkingLocationIdIsRequired,
    WorkingLocationIdDoesNotExist,

    WorkingMachineryLabel,
    WorkingMachineryIdIsRequired,
    WorkingMachineryIdDoesNotExist,
}