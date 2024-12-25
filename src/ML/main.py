import os
from concurrent import futures
import grpc
import ApiGrpc.service_pb2 as service_pb2
import ApiGrpc.service_pb2_grpc as service_pb2_grpc
import joblib
import pandas as pd
import numpy as np

LISTEN_PORT = os.environ.get('LISTEN_PORT', '50051')
MODEL_PATH = os.environ.get('MODEL_PATH', 'Learning/rating_predictor_model.pkl')

class RatingPredictorServicer(service_pb2_grpc.RatingPredictorServicer):
    def __init__(self):
        self.model = joblib.load(MODEL_PATH)
        pass

    def PredictRating(self, request, context):
        # Prepare the input data for prediction
        last_rating = request.ratings[-1].rating
        input_data = np.array([last_rating]).reshape(-1, 1, 1)

        # Predict future ratings
        predictions = []
        for _ in range(request.num_predictions):
            prediction = self.model.predict(input_data)
            input_data = np.append(input_data[:, 1:, :], prediction.reshape(-1, 1, 1), axis=1)
            predictions.append(prediction[0][0])

        # Prepare the response
        response = service_pb2.PredictRatingResponse()
        for i, rating in enumerate(predictions):
            prediction_date = (pd.to_datetime(request.ratings[-1].date) + pd.DateOffset(days=i + 1)).strftime('%Y-%m-%d')
            response.predictions.add(date=prediction_date, rating=rating)

        return response


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    service_pb2_grpc.add_RatingPredictorServicer_to_server(RatingPredictorServicer(), server)
    server.add_insecure_port('[::]:%s' % LISTEN_PORT)
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    print('Starting server. Listening on port %s.' % LISTEN_PORT)
    try:
        serve()
    except Exception as e:
        print('Error: %s' % e)
        raise
