using System;
using System.Collections.Generic;

namespace LightGBM
{
    public enum LGBMDataType
    {
        Float32 = 0,
        Float64 = 1,
        Int32 = 2,
        Int64 = 3
    }

    public enum LGBMPredictType
    {
        PredictNormal = 0,
        PredictRawScore = 1,
        PredictLeafIndex = 2,
        PredictContrib= 3,
    }

    /// <summary>
    /// Valid values fort the metric parameter
    /// <para>
    /// https://github.com/Microsoft/LightGBM/blob/master/docs/Parameters.rst#metric-parameters
    /// </para>
    /// </summary>
    public enum Metric
    {
        l1,
        l2,
        l2_root,
        ndrg,
        auc,
        binary_logloss,
        binary_error,
        multi_logloss,
        multi_error,
    }

    /// <summary>
    /// Valid values for the application parameter
    /// </summary>
    public enum Objective
    {
        regression,
        regression_l2,
        regression_l1,
        huber,
        fair,
        poisson,

        binary,

        lambdarank,

        multiclass,
    }

    public enum TreeLearner
    {
        serial,
        feature,
        data,
    }

    public static class Constants
    {
        public const string LABEL = "label";
        public const string WEIGHT = "weight";
        public const string INIT_SCORE = "init_score";
        public const string GROUP = "group";
        public const string GROUP_ID = "group_id";

        public static Dictionary<string, LGBMDataType> FieldDataTypeMap = new Dictionary<string, LGBMDataType>
        {
            { LABEL, LGBMDataType.Float32 },
            { WEIGHT, LGBMDataType.Float32 },
            { INIT_SCORE, LGBMDataType.Float64 },
            { GROUP, LGBMDataType.Int32 },
            { GROUP_ID, LGBMDataType.Int32 },
        };

        public static Dictionary<string, Type> FieldTypeMap = new Dictionary<string, Type>
        {
            { LABEL, typeof(float) },
            { WEIGHT, typeof(float) },
            { INIT_SCORE, typeof(double) },
            { GROUP, typeof(int) },
            { GROUP_ID, typeof(int) },
        };

        public static Dictionary<Type, LGBMDataType> TypeLgbmTypeMap = new Dictionary<Type, LGBMDataType>
        {
            {typeof(float), LGBMDataType.Float32 },
            {typeof(double), LGBMDataType.Float64 },
            {typeof(int), LGBMDataType.Int32 },
            {typeof(long), LGBMDataType.Int64 },
        };

        public static Dictionary<LGBMDataType, Type> LgbmTypeTypeMap = new Dictionary<LGBMDataType, Type>
        {
            {LGBMDataType.Float32, typeof(float)},
            {LGBMDataType.Float64, typeof(double)},
            {LGBMDataType.Int32, typeof(int)},
            {LGBMDataType.Int64, typeof(long)},
        };

        public static class DataSetParameters
        {
            public const string is_sparse = "is_sparse";
            public const string max_bin = "max_bin";
            public const string data_random_seed = "data_random_seed";
            public const string categorical_feature = "categorical_feature";
            public static readonly string[] Values =
                { is_sparse, max_bin, data_random_seed, categorical_feature };
        }

        public static readonly string[] MaximizeMetrics =
        { "auc", "ndcg", "map" };  // Python ima 'map'

        public static readonly string[] BoostingParameters =
        { "gbdt", "rf", "dart", "goss" };

        public static readonly string[] BoosterParameters = 
        {
// Core parameters
            "config",
            "task",
            "application",
            "boosting",
// "data"
// "valid"
            "num_iterations",
            "learning_rate",
            "num_leaves",
            "tree_learner",
            "num_threads",
            "device",
// Learning control parameters
            "max_depth",
            "min_data_in_leaf",
            "min_sum_hessian_in_leaf",
            "min_hessian",
            "feature_fraction",
            "feature_fraction_seed",
            "bagging_fraction",
            "bagging_freq",
            "bagging_seed",
            "early_stopping_round",
            "lambda_l1",
            "lambda_l2",
            "min_gain_to_split",
            "drop_rate",
            "skip_drop",
            "max_drop",
            "uniform_drop",
            "xgboost_dart_mode",
            "drop_seed",
            "top_rate",
            "other_rate",
            "min_data_per_group",
            "max_cat_threshold",
            "cat_smooth",
            "cat_l2",
            "max_cat_to_onehot",
// Objective parameters
             "sigmoid",
             "huber_delta",
             "fair_c",
             "gaussian_eta",
             "poisson_max_delta_step",
             "scale_pos_weight",
             "boost_from_average",
             "is_unbalance",
             "max_position",
             "label_gain",
             "num_class",
// Metric parameters
            "metric",
            "metric_freq",
            "is_training_metric",
            "ndcg_at",
// Network parameters
            "num_machines",
            "local_listen_port",
            "time_out",
            "machine_list_file",
            "histogram_pool_size",
        };

    }
}
